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
        MaximumLength = maximumLength;
    }
}