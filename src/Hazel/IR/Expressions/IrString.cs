namespace Hazel.IR.Expressions;

public sealed class IrString : IrExpression
{
    public string Value
    {
        get;
    }

    public IrString(
        string value)
    {
        Value = value;
    }
}