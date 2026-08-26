using Hazel.Diagnostics;
using Hazel.Semantics.Types;

namespace Hazel.Syntax.Expressions;

public abstract class Expression
    : AstNode
{
    public TypeSymbol? ResolvedType
    {
        get;
        set;
    }

    protected Expression(SourceSpan span)
        : base(span)
    {
    }
}