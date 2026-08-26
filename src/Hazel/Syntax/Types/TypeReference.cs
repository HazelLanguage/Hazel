using Hazel.Diagnostics;
using Hazel.Syntax.Types;

namespace Hazel.Syntax.Types;

public abstract class TypeReference : AstNode
{
    protected TypeReference(SourceSpan span)
        : base(span)
    {
    }
}