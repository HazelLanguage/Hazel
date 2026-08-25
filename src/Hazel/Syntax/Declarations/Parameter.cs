using Hazel.Diagnostics;
using Hazel.Syntax.Types;

namespace Hazel.Syntax.Declarations;

public sealed class Parameter : AstNode
{
    public TypeReference Type
    {
        get;
    }

    public string Name
    {
        get;
    }

    public Parameter(
        TypeReference type,
        string name,
        SourceSpan span)
        : base(span)
    {
        Type = type;
        Name = name;
    }

    public override T Accept<T>(
        AstVisitor<T> visitor)
    {
        return visitor.VisitParameter(this);
    }
}