namespace Hazel.IR.Types;

public sealed class IrBoundedStringType
    : IrTypeReference
{
    public int MaximumLength
    {
        get;
    }

    public IrBoundedStringType(
        int maximumLength)
    {
        if (maximumLength < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maximumLength));
        }

        MaximumLength = maximumLength;
    }
}