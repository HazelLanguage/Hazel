namespace Hazel.Semantics.Types;

public sealed class UnknownTypeSymbol : TypeSymbol
{
    public override string Name => "Unknown";

    public static readonly UnknownTypeSymbol Instance = new();
}