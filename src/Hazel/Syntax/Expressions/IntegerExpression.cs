using Hazel.Diagnostics;

namespace Hazel.Syntax.Expressions;

public sealed class IntegerExpression : Expression
{
    public long Value
    {
        get;
    }

    public IntegerExpression(
        long value,
        SourceSpan span)
        : base(span)
    {
        Value = value;
    }

    public override T Accept<T>(AstVisitor<T> visitor)
    {
        return visitor.VisitInteger(this);
    }
}