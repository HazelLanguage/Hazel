using Hazel.Diagnostics;

namespace Hazel.Syntax;

public abstract class AstNode
{
    public SourceSpan Span
    {
        get;
    }

    protected AstNode(SourceSpan span)
    {
        Span = span;
    }

    public abstract T Accept<T>(
        AstVisitor<T> visitor);
}