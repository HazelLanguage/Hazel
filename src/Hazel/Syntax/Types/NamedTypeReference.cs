using Hazel.Diagnostics;

namespace Hazel.Syntax.Types;

public sealed class NamedTypeReference : TypeReference
{
    public string Name
    {
        get;
    }

    public NamedTypeReference(
        string name,
        SourceSpan span)
        : base(span)
    {
        Name = name;
    }

    public override T Accept<T>(
        AstVisitor<T> visitor)
    {
        return visitor.VisitNamedTypeReference(this);
    }
}