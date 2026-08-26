using Hazel.Diagnostics;

namespace Hazel.Syntax.Types;

public sealed class BoundedStringTypeReference : TypeReference
{
    public int MaximumLength
    {
        get;
    }

    public BoundedStringTypeReference(
        int maximumLength,
        SourceSpan span)
        : base(span)
    {
        MaximumLength = maximumLength;
    }

    public override T Accept<T>(
        AstVisitor<T> visitor)
    {
        return visitor.VisitBoundedStringTypeReference(this);
    }
}