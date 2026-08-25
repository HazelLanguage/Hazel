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
        foreach (var member in node.Members)
        {
            member.Accept(this);
        }

        return UnknownTypeSymbol.Instance;
    }

    public override TypeSymbol VisitMethod(
        MethodDeclaration node)
    {
        // Enter a new nested scope for the method
        _scope = new Scope(_scope);

        // Define parameters in the method scope
        foreach (var parameter in node.Parameters)
        {
            TypeSymbol paramType = parameter.Type.Accept(this);

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

        // Restore the parent scope when leaving the method
        _scope = _scope.Parent!;

        return UnknownTypeSymbol.Instance;
    }

    public override TypeSymbol VisitParameter(
        Parameter node)
    {
        return UnknownTypeSymbol.Instance;
    }

    public override TypeSymbol VisitTypeReference(
        TypeReference node)
    {
        return node.Name switch
        {
            "int" => IntTypeSymbol.Instance,
            _ => UnknownTypeSymbol.Instance
        };
    }

    public override TypeSymbol VisitReturn(
        ReturnStatement node)
    {
        if (node.Value is null)
            return UnknownTypeSymbol.Instance;

        return node.Value.Accept(this);
    }

    public override TypeSymbol VisitInteger(
        IntegerExpression node)
    {
        return IntTypeSymbol.Instance;
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

        return symbol.Type;
    }

    public override TypeSymbol VisitBinary(
        BinaryExpression node)
    {
        var left =
            node.Left.Accept(this);

        var right =
            node.Right.Accept(this);

        if (left != IntTypeSymbol.Instance ||
            right != IntTypeSymbol.Instance)
        {
            throw new Exception(
                "Binary arithmetic requires integers.");
        }

        return IntTypeSymbol.Instance;
    }

    public override TypeSymbol VisitVariable(
        VariableStatement node)
    {
        TypeSymbol type =
            node.Value.Accept(this);

        var symbol = new Symbol(
            node.Name,
            SymbolKind.Variable,
            type);

        if (!_scope.Define(symbol))
        {
            throw new Exception(
                $"Variable '{node.Name}' " +
                "is already defined.");
        }

        return type;
    }

    public override TypeSymbol VisitExpressionStatement(
        ExpressionStatement node)
    {
        return node.Expression.Accept(this);
    }
}