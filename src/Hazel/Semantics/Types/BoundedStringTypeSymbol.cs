namespace Hazel.Semantics.Types;

public sealed class BoundedStringTypeSymbol : TypeSymbol
{
    public int MaximumLength
    {
        get;
    }

    public override string Name =>
        $"text[{MaximumLength}]";

    public BoundedStringTypeSymbol(
        int maximumLength)
    {
        MaximumLength = maximumLength;
    }
}