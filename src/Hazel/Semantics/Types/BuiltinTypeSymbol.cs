using Hazel.Semantics.Types;

public sealed class BuiltinTypeSymbol : TypeSymbol
{
    public override string Name
    {
        get;
    }

    public int? BitWidth
    {
        get;
    }

    public bool? IsSigned
    {
        get;
    }

    public BuiltinTypeSymbol(
        string name,
        int? bitWidth = null,
        bool? isSigned = null)
    {
        Name = name;
        BitWidth = bitWidth;
        IsSigned = isSigned;
    }
}