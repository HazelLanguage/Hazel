using Hazel.Semantics.Types;

public sealed class StringTypeSymbol : TypeSymbol
{
    public static readonly StringTypeSymbol Instance = new();

    public override string Name => "string";

    private StringTypeSymbol()
    {
    }
}