namespace Hazel.IR.Statements;

public sealed class IrReturnStatement
    : IrStatement
{
    public IrExpression? Expression
    {
        get;
    }

    public IrReturnStatement(
        IrExpression? expression)
    {
        Expression = expression;
    }
}