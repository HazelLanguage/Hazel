namespace Hazel.IR.Expressions;

using Hazel.IR.Types;

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

    public override IrValueType Type =>
        new IrBoundedStringType(MaximumLength);

    public IrBoundedString(
        string value,
        int maximumLength)
    {
        if (maximumLength <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maximumLength));
        }

        if (value.Length > maximumLength)
        {
            throw new ArgumentException(
                "Bounded string exceeds maximum length.",
                nameof(value));
        }

        Value = value;
        MaximumLength = maximumLength;
    }
}