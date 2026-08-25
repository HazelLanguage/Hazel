using Hazel.Diagnostics;

namespace Hazel.Syntax.Statements;

public abstract class Statement : AstNode
{
    protected Statement(SourceSpan span)
        : base(span)
    {
    }
}