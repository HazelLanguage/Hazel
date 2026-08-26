namespace Hazel.Semantics.Types;

public sealed class IntTypeSymbol : TypeSymbol
{
    public static readonly IntTypeSymbol Instance = new();

    public override string Name => "integer";

    private IntTypeSymbol()
    {
    }
}