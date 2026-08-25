using Hazel.Diagnostics;
using Hazel.Syntax.Expressions;

namespace Hazel.Syntax.Statements;

public sealed class ReturnStatement : Statement
{
    public Expression? Value
    {
        get;
    }

    public ReturnStatement(
        Expression? value,
        SourceSpan span)
        : base(span)
    {
        Value = value;
    }

    public override T Accept<T>(
        AstVisitor<T> visitor)
    {
        return visitor.VisitReturn(this);
    }
}