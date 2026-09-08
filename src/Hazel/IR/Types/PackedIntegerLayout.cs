namespace Hazel.IR.Types;

public sealed class PackedField
{
    public string Name
    {
        get;
    }

    public IrIntegerType Type
    {
        get;
    }

    public int BitOffset
    {
        get;
    }

    public int BitWidth => Type.BitWidth;

    public int StorageUnitIndex => BitOffset / StorageUnitBits;

    public int StorageUnitBitOffset => BitOffset % StorageUnitBits;

    public int StorageUnitBits
    {
        get;
    }

    public int ValueMask =>
        (1 << BitWidth) - 1;

    public int StorageMask =>
        ValueMask << StorageUnitBitOffset;

    public PackedField(
        string name,
        IrIntegerType type,
        int bitOffset,
        int storageUnitBits)
    {
        Name = name;
        Type = type;
        BitOffset = bitOffset;
        StorageUnitBits = storageUnitBits;
    }
}

public sealed class PackedStorageLayout
{
    public int StorageUnitBits
    {
        get;
    }

    public List<PackedField> Fields
    {
        get;
    } = new();

    public int TotalBitCount
    {
        get;
        private set;
    }

    public int StorageUnitCount =>
        (TotalBitCount + StorageUnitBits - 1) / StorageUnitBits;

    public PackedStorageLayout(
        int storageUnitBits = 8)
    {
        if (storageUnitBits is not (8 or 16 or 32 or 64 or 128))
        {
            throw new ArgumentOutOfRangeException(
                nameof(storageUnitBits),
                "Storage unit size must be one of 8, 16, 32, 64, or 128 bits.");
        }

        StorageUnitBits = storageUnitBits;
    }

    public void AddField(
        string name,
        IrIntegerType type)
    {
        int offsetInUnit =
            TotalBitCount % StorageUnitBits;

        if (offsetInUnit + type.BitWidth > StorageUnitBits)
        {
            TotalBitCount +=
                StorageUnitBits - offsetInUnit;
        }

        Fields.Add(
            new PackedField(
                name,
                type,
                TotalBitCount,
                StorageUnitBits));

        TotalBitCount += type.BitWidth;
    }

    public static string GetStorageUnitTypeName(int storageUnitBits)
    {
        return storageUnitBits switch
        {
            8 => "byte",
            16 => "ushort",
            32 => "uint",
            64 => "ulong",
            128 => "System.UInt128",
            _ => throw new ArgumentOutOfRangeException(
                nameof(storageUnitBits))
        };
    }

    public static PackedStorageLayout Pack(
        params (string Name, IrIntegerType Type)[] fields)
    {
        var layout = new PackedStorageLayout();

        foreach (var field in fields)
        {
            layout.AddField(field.Name, field.Type);
        }

        return layout;
    }
}
