using Hazel.IR;
using Hazel.IR.Expressions;
using Hazel.IR.Statements;
using Hazel.Syntax;
using Hazel.Syntax.Declarations;
using Hazel.Syntax.Expressions;
using Hazel.Syntax.Statements;

namespace Hazel.Lowering;

public sealed class AstToIrLowerer
    : AstVisitor<IrNode>
{
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
            if (declaration is NamespaceDeclaration namespaceDeclaration)
            {
                var irNamespace = (IrNamespace)LowerNamespace(namespaceDeclaration);
                program.Namespaces.Add(irNamespace);
            }
            else if (declaration is TypeDeclaration typeDeclaration)
            {
                throw new NotSupportedException("Types must be inside a namespace.");
            }
            else
            {
                throw new NotSupportedException(
                    $"Cannot lower declaration {declaration.GetType().Name}");
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
        var irType = new IrType(node.AccessModifiers, node.Name, node.Kind);

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
        var irMethod = new IrMethod(node.AccessModifiers, node.Name, node.ReturnType.Name);

        foreach (var param in node.Parameters)
        {
            irMethod.Parameters.Add(new IrParameter(param.Name, param.Type.Name));
        }

        foreach (var statement in node.Body)
        {
            irMethod.Body.Add(
                (IrStatement)statement.Accept(this));
        }

        return irMethod;
    }

    public override IrNode VisitInteger(
        IntegerExpression node)
    {
        return new IrConstant(node.Value);
    }

    public override IrNode VisitIdentifier(
        IdentifierExpression node)
    {
        return new IrVariable(node.Name);
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
        var value =
            (IrExpression)node.Value.Accept(this);

        return new IrVariableDeclaration(
            node.Name,
            value);
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
        var expression = node.Value != null
            ? (IrExpression)node.Value.Accept(this)
            : null;

        return new IrReturnStatement(expression);
    }
}