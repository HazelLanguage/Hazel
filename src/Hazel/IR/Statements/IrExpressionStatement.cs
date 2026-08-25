namespace Hazel.IR.Statements;

public sealed class IrExpressionStatement
    : IrStatement
{
    public IrExpression Expression
    {
        get;
    }

    public IrExpressionStatement(
        IrExpression expression)
    {
        Expression = expression;
    }
}