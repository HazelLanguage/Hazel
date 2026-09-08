namespace Hazel.Semantics.Types;

public static class BuiltinTypes
{
    public static readonly TypeSymbol Void =
        new BuiltinTypeSymbol("void");

    public static readonly TypeSymbol Dynamic =
        new BuiltinTypeSymbol("dynamic");

    public static readonly TypeSymbol String =
        new BuiltinTypeSymbol("string");

    public static readonly TypeSymbol Character =
        new BuiltinTypeSymbol("character");

    public static readonly TypeSymbol Integer8 =
        new BuiltinTypeSymbol("integer8", 8, true);

    public static readonly TypeSymbol UnsignedInteger8 =
        new BuiltinTypeSymbol("uinteger8", 8, false);

    public static readonly TypeSymbol Integer16 =
        new BuiltinTypeSymbol("integer16", 16, true);

    public static readonly TypeSymbol UnsignedInteger16 =
        new BuiltinTypeSymbol("uinteger16", 16, false);

    public static readonly TypeSymbol Integer32 =
        new BuiltinTypeSymbol("integer32", 32, true);

    public static readonly TypeSymbol UnsignedInteger32 =
        new BuiltinTypeSymbol("uinteger32", 32, false);

    public static readonly TypeSymbol Integer64 =
        new BuiltinTypeSymbol("integer64", 64, true);

    public static readonly TypeSymbol UnsignedInteger64 =
        new BuiltinTypeSymbol("uinteger64", 64, false);

    public static readonly TypeSymbol Integer128 =
        new BuiltinTypeSymbol("integer128", 128, true);

    public static readonly TypeSymbol UnsignedInteger128 =
        new BuiltinTypeSymbol("uinteger128", 128, false);

    private static readonly Dictionary<string, TypeSymbol> _types = new();

    static BuiltinTypes()
    {
        Add("void", Void);
        Add("dynamic", Dynamic);
        Add("string", String);
        Add("character", Character);

        Add("integer8", Integer8);
        Add("uinteger8", UnsignedInteger8);
        Add("integer16", Integer16);
        Add("uinteger16", UnsignedInteger16);
        Add("integer32", Integer32);
        Add("uinteger32", UnsignedInteger32);
        Add("integer64", Integer64);
        Add("uinteger64", UnsignedInteger64);
        Add("integer128", Integer128);
        Add("uinteger128", UnsignedInteger128);

        for (int bitWidth = 1; bitWidth <= 128; bitWidth++)
        {
            Add($"integer{bitWidth}", new BuiltinTypeSymbol($"integer{bitWidth}", bitWidth, true));
            Add($"uinteger{bitWidth}", new BuiltinTypeSymbol($"uinteger{bitWidth}", bitWidth, false));
        }
    }

    private static void Add(string name, TypeSymbol type)
    {
        _types[name] = type;
    }

    public static TypeSymbol Get(string name)
    {
        if (_types.TryGetValue(name, out var type))
            return type;

        throw new Exception(
            $"Unknown type '{name}'.");
    }
}
