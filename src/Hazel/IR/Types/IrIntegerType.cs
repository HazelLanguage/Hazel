using System.Numerics;

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

    public bool IsSubByte => BitWidth > 0 && BitWidth < 8;

    public IrIntegerType(
        int bitWidth,
        bool isSigned)
    {
        if (bitWidth <= 0 || bitWidth > 128)
        {
            throw new ArgumentOutOfRangeException(
                nameof(bitWidth),
                "Bit width must be between 1 and 128.");
        }

        BitWidth = bitWidth;
        IsSigned = isSigned;
    }

    public static BigInteger GetMinimumValue(
        int bitWidth,
        bool isSigned)
    {
        if (isSigned)
        {
            return -(BigInteger.One << (bitWidth - 1));
        }

        return BigInteger.Zero;
    }

    public static BigInteger GetMaximumValue(
        int bitWidth,
        bool isSigned)
    {
        if (isSigned)
        {
            return (BigInteger.One << (bitWidth - 1)) - 1;
        }

        return (BigInteger.One << bitWidth) - 1;
    }

    public static bool IsInRange(
        BigInteger value,
        int bitWidth,
        bool isSigned)
    {
        return value >= GetMinimumValue(bitWidth, isSigned) &&
               value <= GetMaximumValue(bitWidth, isSigned);
    }

    public static bool IsInRange(
        long value,
        int bitWidth,
        bool isSigned)
    {
        return IsInRange(
            new BigInteger(value),
            bitWidth,
            isSigned);
    }

    public static int GetStorageUnitBits(
        int bitWidth)
    {
        if (bitWidth <= 8)
            return 8;

        if (bitWidth <= 16)
            return 16;

        if (bitWidth <= 32)
            return 32;

        if (bitWidth <= 64)
            return 64;

        return 128;
    }
}
