namespace Hazel.Semantics.Types;

public sealed class TextTypeSymbol : TypeSymbol
{
    public static readonly TextTypeSymbol Instance = new();

    public override string Name => "text";

    private TextTypeSymbol()
    {
    }
}