namespace Hazel.IR.Expressions;

public sealed class IrBoundedString
    : IrExpression
{
    public string Value
    {
        get;
    }

    public int MaximumLength
    {
        get;
    }

    public IrBoundedString(
        string value,
        int maximumLength)
    {
        Value = value;
        MaximumLength = maximumLength;
    }
}