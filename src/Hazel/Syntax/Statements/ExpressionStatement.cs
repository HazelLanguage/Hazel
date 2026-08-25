using Hazel.Diagnostics;
using Hazel.Syntax.Expressions;

namespace Hazel.Syntax.Statements;

public sealed class ExpressionStatement : Statement
{
    public Expression Expression
    {
        get;
    }

    public ExpressionStatement(
        Expression expression,
        SourceSpan span)
        : base(span)
    {
        Expression = expression;
    }

    public override T Accept<T>(AstVisitor<T> visitor)
    {
        return visitor.VisitExpressionStatement(this);
    }
}