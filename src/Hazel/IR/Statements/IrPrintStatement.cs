namespace Hazel.IR.Statements;

public sealed class IrPrintStatement : IrStatement
{
    public IrExpression Expression
    {
        get;
    }

    public IrPrintStatement(IrExpression expression)
    {
        Expression = expression;
    }
}