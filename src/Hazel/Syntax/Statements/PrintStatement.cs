using Hazel.Diagnostics;
using Hazel.Syntax.Expressions;

namespace Hazel.Syntax.Statements;

public sealed class PrintStatement : Statement
{
    public Expression Expression
    {
        get;
    }

    public PrintStatement(
        Expression expression,
        SourceSpan span)
        : base(span)
    {
        Expression = expression;
    }

    public override T Accept<T>(AstVisitor<T> visitor)
    {
        return visitor.VisitPrint(this);
    }
}