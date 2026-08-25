namespace Hazel.IR.Expressions;

public sealed class IrConstant : IrExpression
{
    public long Value
    {
        get;
    }

    public IrConstant(long value)
    {
        Value = value;
    }
}