namespace Hazel.Semantics.Types;

public sealed class IntTypeSymbol : TypeSymbol
{
    public override string Name => "Int";

    public static readonly IntTypeSymbol Instance = new();
}