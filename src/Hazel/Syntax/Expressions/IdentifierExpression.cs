using Hazel.Diagnostics;

namespace Hazel.Syntax.Expressions;

public sealed class IdentifierExpression : Expression
{
    public string Name
    {
        get;
    }

    public IdentifierExpression(
        string name,
        SourceSpan span)
        : base(span)
    {
        Name = name;
    }

    public override T Accept<T>(AstVisitor<T> visitor)
    {
        return visitor.VisitIdentifier(this);
    }
}