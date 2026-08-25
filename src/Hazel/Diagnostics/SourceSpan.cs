namespace Hazel.Diagnostics;

public readonly record struct SourceSpan(
    int Start,
    int Length)
{
    public int End => Start + Length;

    public static SourceSpan FromBounds(
        int start,
        int end)
    {
        return new SourceSpan(
            start,
            end - start);
    }
}