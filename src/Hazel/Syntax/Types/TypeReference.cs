using Hazel.Diagnostics;

namespace Hazel.Syntax.Types;

public sealed class TypeReference : AstNode
{
    public string Name
    {
        get;
    }

    public TypeReference(
        string name,
        SourceSpan span)
        : base(span)
    {
        Name = name;
    }

    public override T Accept<T>(
        AstVisitor<T> visitor)
    {
        return visitor.VisitTypeReference(this);
    }
}