using System.Reflection.Metadata;
using Hazel.IR;
using Hazel.IR.Expressions;
using Hazel.IR.Statements;
using Hazel.IR.Types;
using Hazel.Semantics;
using Hazel.Semantics.Types;
using Hazel.Syntax;
using Hazel.Syntax.Declarations;
using Hazel.Syntax.Expressions;
using Hazel.Syntax.Imports;
using Hazel.Syntax.Statements;
using Hazel.Syntax.Types;
using Microsoft.VisualBasic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Hazel.Lowering;

public sealed class AstToIrLowerer
    : AstVisitor<IrNode>
{
    private IrTypeReference? _currentReturnType;

    public IrProgram Lower(
        CompilationUnit compilationUnit)
    {
        return (IrProgram)compilationUnit.Accept(this);
    }

    public override IrNode VisitCompilationUnit(
        CompilationUnit node)
    {
        var program = new IrProgram();

        foreach (var declaration in node.Declarations)
        {
            switch (declaration)
            {
                case ImportDeclaration import:
                    program.ImportedLibraries.Add(import.Name);
                    break;

                case NamespaceDeclaration namespaceDeclaration:
                    var irNamespace =
                        (IrNamespace)LowerNamespace(namespaceDeclaration);

                    program.Namespaces.Add(irNamespace);
                    break;

                case TypeDeclaration:
                    throw new NotSupportedException(
                        "Types must be inside a namespace.");

                default:
                    throw new NotSupportedException(
                        $"Cannot lower declaration " +
                        $"{declaration.GetType().Name}");
            }
        }

        return program;
    }

    public IrNode LowerNamespace(
        NamespaceDeclaration node)
    {
        var irNamespace = new IrNamespace(node.Name);

        foreach (var member in node.Members)
        {
            if (member is TypeDeclaration typeDeclaration)
            {
                var irType = (IrType)LowerType(typeDeclaration);
                irNamespace.Types.Add(irType);
            }
            else
            {
                throw new NotSupportedException(
                    $"Cannot lower namespace member {member.GetType().Name}");
            }
        }

        return irNamespace;
    }

    private IrNode LowerType(
        TypeDeclaration node)
    {
        var irType = new IrType(
            node.AccessModifiers,
            node.Modifiers,
            node.Name,
            node.Kind);

        foreach (var member in node.Members)
        {
            if (member is MethodDeclaration method)
            {
                var irMethod = (IrMethod)LowerMethod(method);
                irType.Methods.Add(irMethod);
            }
        }

        return irType;
    }

    public IrNode LowerMethod(
    MethodDeclaration node)
    {
        var returnType =
            (IrTypeReference)node.ReturnType.Accept(this);

        var previousReturnType =
            _currentReturnType;

        _currentReturnType =
            returnType;

        try
        {
            var irMethod = new IrMethod(
                node.AccessModifiers,
                node.Name,
                returnType);

            foreach (var param in node.Parameters)
            {
                irMethod.Parameters.Add(
                    new IrParameter(
                        param.Name,
                        (IrTypeReference)param.Type.Accept(this)));
            }

            foreach (var statement in node.Body)
            {
                irMethod.Body.Add(
                    (IrStatement)statement.Accept(this));
            }

            return irMethod;
        }
        finally
        {
            _currentReturnType =
                previousReturnType;
        }
    }

    private IrExpression LowerExpression(
    Expression expression,
    IrTypeReference? expectedType)
    {
        if (expression is StringExpression stringExpression &&
            expectedType is IrBoundedStringType boundedString)
        {
            return new IrBoundedString(
                stringExpression.Value,
                boundedString.MaximumLength);
        }

        return (IrExpression)expression.Accept(this);
    }

    private static IrValueType LowerValueType(
    TypeSymbol type)
    {
        return type switch
        {
            BoundedStringTypeSymbol bounded =>
                new IrBoundedStringType(
                    bounded.MaximumLength),

            BuiltinTypeSymbol { Name: "string" } =>
                IrStringType.Instance,

            BuiltinTypeSymbol builtin
                when builtin.BitWidth is int bitWidth &&
                     builtin.IsSigned is bool isSigned =>
                new IrIntegerType(
                    bitWidth,
                    isSigned),

            _ =>
                throw new NotSupportedException(
                    $"Cannot lower semantic type " +
                    $"{type.GetType().Name}.")
        };
    }

    public override IrNode VisitInteger(
        IntegerExpression node)
    {
        return new IrConstant(node.Value);
    }

    public override IrNode VisitIdentifier(
    IdentifierExpression node)
    {
        if (node.ResolvedType == null)
        {
            throw new InvalidOperationException(
                $"Identifier '{node.Name}' " +
                "has not been semantically resolved.");
        }

        return new IrVariable(
            node.Name,
            LowerValueType(node.ResolvedType));
    }

    public override IrNode VisitConversionExpression(
        ConversionExpression node)
    {
        if (node.ResolvedType is not BoundedStringTypeSymbol targetType)
        {
            throw new InvalidOperationException(
                "Conversion expression has not been " +
                "semantically resolved as a bounded string.");
        }

        var value =
            (IrExpression)node.Value.Accept(this);

        return new IrBoundedStringConversion(
            value,
            targetType.MaximumLength);
    }

    public override IrNode VisitBinary(
        BinaryExpression node)
    {
        var left =
            (IrExpression)node.Left.Accept(this);

        var right =
            (IrExpression)node.Right.Accept(this);

        string op = node.Operator switch
        {
            BinaryOperator.Add => "+",
            BinaryOperator.Subtract => "-",
            BinaryOperator.Multiply => "*",
            BinaryOperator.Divide => "/",

            _ => throw new Exception()
        };

        return new IrBinary(
            left,
            op,
            right);
    }

    public override IrNode VisitVariable(
    VariableStatement node)
    {
        var type =
            (IrTypeReference)node.Type.Accept(this);

        var value =
            LowerExpression(
                node.Value,
                type);

        return new IrVariableDeclaration(
            node.Name,
            type,
            value);
    }

    public override IrNode VisitString(
    StringExpression node)
    {
        return new IrString(node.Value);
    }

    public override IrNode VisitExpressionStatement(
        ExpressionStatement node)
    {
        var expression =
            (IrExpression)node.Expression.Accept(this);

        return new IrExpressionStatement(
            expression);
    }

    public override IrNode VisitReturn(
    ReturnStatement node)
    {
        IrExpression? expression = null;

        if (node.Value != null)
        {
            expression =
                LowerExpression(
                    node.Value,
                    _currentReturnType);
        }

        return new IrReturnStatement(
            expression);
    }

    public override IrNode VisitNamedTypeReference(
    NamedTypeReference node)
    {
        return new IrNamedType(node.Name);
    }

    public override IrNode VisitBoundedStringTypeReference(
        BoundedStringTypeReference node)
    {
        return new IrBoundedStringType(
            node.MaximumLength);
    }
}