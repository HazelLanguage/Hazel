namespace Hazel.IR.Types;

public sealed class IrIntegerType
    : IrValueType
{
    public int BitWidth
    {
        get;
    }

    public bool IsSigned
    {
        get;
    }

    public IrIntegerType(
        int bitWidth,
        bool isSigned)
    {
        BitWidth = bitWidth;
        IsSigned = isSigned;
    }
}