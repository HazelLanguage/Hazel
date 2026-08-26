using Hazel.Diagnostics;

namespace Hazel.Syntax.Expressions;

public sealed class StringExpression : Expression
{
    public string Value
    {
        get;
    }

    public StringExpression(
        string value,
        SourceSpan span)
        : base(span)
    {
        Value = value;
    }

    public override T Accept<T>(
        AstVisitor<T> visitor)
    {
        return visitor.VisitString(this);
    }
}