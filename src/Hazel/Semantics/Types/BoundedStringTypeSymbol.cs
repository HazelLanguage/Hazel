namespace Hazel.Semantics.Types;

public sealed class BoundedStringTypeSymbol
    : TypeSymbol
{
    public int MaximumLength
    {
        get;
    }

    public override string Name =>
        $"string[{MaximumLength}]";

    public BoundedStringTypeSymbol(
        int maximumLength)
    {
        MaximumLength = maximumLength;
    }

    public override bool Equals(object? obj)
    {
        if (obj is BoundedStringTypeSymbol other)
        {
            return MaximumLength == other.MaximumLength;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return MaximumLength.GetHashCode();
    }

    public static bool operator ==(BoundedStringTypeSymbol? left, BoundedStringTypeSymbol? right)
    {
        if (ReferenceEquals(left, right))
            return true;
        if (left is null || right is null)
            return false;
        return left.MaximumLength == right.MaximumLength;
    }

    public static bool operator !=(BoundedStringTypeSymbol? left, BoundedStringTypeSymbol? right)
    {
        return !(left == right);
    }
}
