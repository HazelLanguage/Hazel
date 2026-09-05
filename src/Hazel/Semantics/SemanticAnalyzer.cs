using Hazel.Semantics.Types;
using Hazel.Syntax;
using Hazel.Syntax.Declarations;
using Hazel.Syntax.Expressions;
using Hazel.Syntax.Statements;
using Hazel.Syntax.Types;

namespace Hazel.Semantics;

public sealed class SemanticAnalyzer
    : AstVisitor<TypeSymbol>
{
    private Scope _scope = new();
    private TypeSymbol? _currentReturnType;

    private static bool AreAssignable(
    TypeSymbol source,
    TypeSymbol destination)
    {
        // Exact same type.
        if (source.Equals(destination))
            return true;

        // bounded string -> string
        if (source is BoundedStringTypeSymbol &&
            destination == BuiltinTypes.String)
        {
            return true;
        }

        // bounded string[N] -> bounded string[M]
        // These conversions must now be explicit using (type)value syntax
        // No implicit conversions allowed (neither widening nor narrowing)

        return false;
    }

    private static bool IsStringLiteralWithinBounds(
    StringExpression expression,
    BoundedStringTypeSymbol destination)
    {
        return expression.Value.Length <=
               destination.MaximumLength;
    }

    public void Analyze(
    CompilationUnit compilationUnit)
    {
        compilationUnit.Accept(this);
    }

    public override TypeSymbol VisitCompilationUnit(
        CompilationUnit node)
    {
        foreach (var declaration in node.Declarations)
        {
            declaration.Accept(this);
        }

        return UnknownTypeSymbol.Instance;
    }

    public override TypeSymbol VisitNamespace(
        NamespaceDeclaration node)
    {
        foreach (var member in node.Members)
        {
            member.Accept(this);
        }

        return UnknownTypeSymbol.Instance;
    }

    public override TypeSymbol VisitType(
        TypeDeclaration node)
    {
        if (node.Modifiers.HasFlag(TypeModifiers.Sealed) &&
            node.Kind != TypeKind.Class)
        {
            throw new Exception(
                "The 'sealed' modifier can only be applied to classes.");
        }

        if (node.Modifiers.HasFlag(TypeModifiers.Abstract) &&
            node.Kind != TypeKind.Class)
        {
            throw new Exception(
                "The 'abstract' modifier can only be applied to classes.");
        }

        foreach (var member in node.Members)
        {
            member.Accept(this);
        }

        return UnknownTypeSymbol.Instance;
    }

    public override TypeSymbol VisitMethod(
    MethodDeclaration node)
    {
        TypeSymbol returnType =
            node.ReturnType.Accept(this);

        var previousReturnType =
            _currentReturnType;

        _currentReturnType = returnType;

        _scope = new Scope(_scope);

        foreach (var parameter in node.Parameters)
        {
            TypeSymbol paramType =
                parameter.Type.Accept(this);

            var symbol = new Symbol(
                parameter.Name,
                SymbolKind.Parameter,
                paramType);

            if (!_scope.Define(symbol))
            {
                throw new Exception(
                    $"Parameter '{parameter.Name}' is already defined.");
            }
        }

        foreach (var statement in node.Body)
        {
            statement.Accept(this);
        }

        _scope = _scope.Parent!;
        _currentReturnType = previousReturnType;

        return UnknownTypeSymbol.Instance;
    }

    public override TypeSymbol VisitParameter(
        Parameter node)
    {
        return UnknownTypeSymbol.Instance;
    }

    public override TypeSymbol VisitNamedTypeReference(
    NamedTypeReference node)
    {
        return BuiltinTypes.Get(node.Name);
    }

    public override TypeSymbol VisitBoundedStringTypeReference(
    BoundedStringTypeReference node)
    {
        if (node.MaximumLength <= 0)
        {
            throw new Exception(
                "Bounded string length must be greater than zero.");
        }

        return new BoundedStringTypeSymbol(
            node.MaximumLength);
    }

    public override TypeSymbol VisitReturn(
    ReturnStatement node)
    {
        TypeSymbol actualType;

        if (node.Value is null)
        {
            actualType = UnknownTypeSymbol.Instance;
        }
        else
        {
            actualType = node.Value.Accept(this);
        }

        if (_currentReturnType is null)
        {
            throw new Exception(
                "Return statement is not inside a method.");
        }

        if (_currentReturnType == BuiltinTypes.Void)
        {
            if (node.Value is not null)
            {
                throw new Exception(
                    "Cannot return a value from a void method.");
            }
            return BuiltinTypes.Void;
        }

        if (node.Value is null)
        {
            throw new Exception(
                $"Method must return '{_currentReturnType.Name}'.");
        }

        if (!AreAssignable(
                actualType,
                _currentReturnType))
        {
            throw new Exception(
                $"Cannot return value of type " +
                $"'{actualType.Name}' from method " +
                $"returning '{_currentReturnType.Name}'.");
        }

        return actualType;
    }

    public override TypeSymbol VisitInteger(
    IntegerExpression node)
    {
        node.ResolvedType =
            BuiltinTypes.Integer32;

        return node.ResolvedType;
    }

    public override TypeSymbol VisitIdentifier(
    IdentifierExpression node)
    {
        var symbol =
            _scope.Lookup(node.Name);

        if (symbol is null)
        {
            throw new Exception(
                $"Undefined variable '{node.Name}'");
        }

        node.ResolvedType = symbol.Type;

        return symbol.Type;
    }

    public override TypeSymbol VisitConversionExpression(
        ConversionExpression node)
    {
        // Resolve the target type
        TypeSymbol targetType = node.TargetType.Accept(this);

        // Resolve the source value type
        TypeSymbol sourceType = node.Value.Accept(this);

        // Validate that the conversion is allowed
        // For explicit conversions with (type)value syntax:
        // 1. bounded string[N] to bounded string[M] - always allowed (explicit)
        // 2. bounded string to string - allowed (explicit)
        // 3. string to bounded string[N] - allowed only if source is string literal that fits

        if (targetType is BoundedStringTypeSymbol targetBounded &&
            sourceType is BoundedStringTypeSymbol sourceBounded)
        {
            // bounded string[N] to bounded string[M]
            // This is always allowed with explicit cast syntax
            node.ResolvedType = targetBounded;
            return targetBounded;
        }

        if (targetType == BuiltinTypes.String &&
            sourceType is BoundedStringTypeSymbol)
        {
            // bounded string to string - allowed
            node.ResolvedType = targetType;
            return targetType;
        }

        if (targetType is BoundedStringTypeSymbol targetBoundedStr &&
            sourceType == BuiltinTypes.String &&
            node.Value is StringExpression stringExpression)
        {
            // string literal to bounded string - check if it fits
            if (stringExpression.Value.Length <= targetBoundedStr.MaximumLength)
            {
                node.ResolvedType = targetBoundedStr;
                return targetBoundedStr;
            }

            throw new Exception(
                $"String literal is {stringExpression.Value.Length} " +
                $"characters long, but target type has a maximum " +
                $"length of {targetBoundedStr.MaximumLength}.");
        }

        throw new Exception(
            $"Cannot convert from '{sourceType.Name}' to " +
            $"'{targetType.Name}' using explicit cast syntax.");
    }

    private static bool IsIntegerType(TypeSymbol type)
    {
        return type is BuiltinTypeSymbol builtin &&
               builtin.BitWidth != null &&
               builtin.IsSigned != null;
    }

    public override TypeSymbol VisitBinary(
        BinaryExpression node)
    {
        var left =
            node.Left.Accept(this);

        var right =
            node.Right.Accept(this);

        if (!IsIntegerType(left) || !IsIntegerType(right))
        {
            throw new Exception(
                "Binary arithmetic requires integers.");
        }

        if (left != right)
        {
            throw new Exception(
                $"Cannot perform arithmetic between different integer types " +
                $"'{left.Name}' and '{right.Name}'.");
        }

        node.ResolvedType = left;

        return left;
    }

    public override TypeSymbol VisitVariable(
    VariableStatement node)
    {
        TypeSymbol declaredType =
            node.Type.Accept(this);

        if (declaredType == BuiltinTypes.Void)
        {
            throw new Exception(
                "Variables cannot have type 'void'.");
        }

        TypeSymbol valueType =
            node.Value.Accept(this);

        if (declaredType is BoundedStringTypeSymbol bounded &&
            valueType == BuiltinTypes.String &&
            node.Value is StringExpression stringExpression)
        {
            if (stringExpression.Value.Length >
                bounded.MaximumLength)
            {
                throw new Exception(
                    $"String literal is {stringExpression.Value.Length} " +
                    $"characters long, but '{node.Name}' has a maximum " +
                    $"length of {bounded.MaximumLength}.");
            }
        }
        else if (!AreAssignable(
                     valueType,
                     declaredType))
        {
            throw new Exception(
                $"Cannot assign value of type " +
                $"'{valueType.Name}' to variable " +
                $"'{node.Name}' of type " +
                $"'{declaredType.Name}'.");
        }

        var symbol = new Symbol(
            node.Name,
            SymbolKind.Variable,
            declaredType);

        if (!_scope.Define(symbol))
        {
            throw new Exception(
                $"Variable '{node.Name}' " +
                "is already defined.");
        }

        return declaredType;
    }

    public override TypeSymbol VisitString(
    StringExpression node)
    {
        node.ResolvedType =
            BuiltinTypes.String;

        return node.ResolvedType;
    }

    public override TypeSymbol VisitExpressionStatement(
        ExpressionStatement node)
    {
        return node.Expression.Accept(this);
    }
}