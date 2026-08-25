using Hazel.Diagnostics;

namespace Hazel.Syntax.Expressions;

public enum BinaryOperator
{
    Add,
    Subtract,
    Multiply,
    Divide
}

public sealed class BinaryExpression : Expression
{
    public Expression Left
    {
        get;
    }
    public BinaryOperator Operator
    {
        get;
    }
    public Expression Right
    {
        get;
    }

    public BinaryExpression(
        Expression left,
        BinaryOperator @operator,
        Expression right,
        SourceSpan span)
        : base(span)
    {
        Left = left;
        Operator = @operator;
        Right = right;
    }

    public override T Accept<T>(AstVisitor<T> visitor)
    {
        return visitor.VisitBinary(this);
    }
}