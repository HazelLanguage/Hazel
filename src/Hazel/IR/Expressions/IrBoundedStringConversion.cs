namespace Hazel.IR.Expressions;

using Hazel.IR.Types;

public sealed class IrBoundedStringConversion
    : IrExpression
{
    public IrExpression Value
    {
        get;
    }

    public int TargetMaximumLength
    {
        get;
    }

    public override IrValueType Type =>
        new IrBoundedStringType(
            TargetMaximumLength);

    public IrBoundedStringConversion(
        IrExpression value,
        int targetMaximumLength)
    {
        if (targetMaximumLength <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(targetMaximumLength));
        }

        Value = value;
        TargetMaximumLength = targetMaximumLength;
    }
}