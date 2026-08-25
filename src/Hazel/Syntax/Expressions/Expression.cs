using Hazel.Diagnostics;

namespace Hazel.Syntax.Expressions;

public abstract class Expression : AstNode
{
    protected Expression(SourceSpan span)
        : base(span)
    {
    }
}