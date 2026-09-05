using Hazel.Diagnostics;
using Hazel.Semantics.Types;
using Hazel.Syntax.Types;

namespace Hazel.Syntax.Expressions;

public sealed class ConversionExpression : Expression
{
    public TypeReference TargetType
    {
        get;
    }

    public Expression Value
    {
        get;
    }

    public ConversionExpression(
        TypeReference targetType,
        Expression value,
        SourceSpan span)
        : base(span)
    {
        TargetType = targetType;
        Value = value;
    }

    public override T Accept<T>(
        AstVisitor<T> visitor)
    {
        return visitor.VisitConversionExpression(this);
    }
}