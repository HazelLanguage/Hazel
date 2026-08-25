using Hazel.Diagnostics;

namespace Hazel.Syntax.Declarations;

public abstract class Declaration : AstNode
{
    protected Declaration(SourceSpan span)
        : base(span)
    {
    }
}