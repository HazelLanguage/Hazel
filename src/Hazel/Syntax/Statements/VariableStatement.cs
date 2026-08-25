using Hazel.Diagnostics;
using Hazel.Syntax.Expressions;

namespace Hazel.Syntax.Statements;

public sealed class VariableStatement : Statement
{
    public string Name
    {
        get;
    }
    public Expression Value
    {
        get;
    }

    public VariableStatement(
        string name,
        Expression value,
        SourceSpan span)
        : base(span)
    {
        Name = name;
        Value = value;
    }

    public override T Accept<T>(AstVisitor<T> visitor)
    {
        return visitor.VisitVariable(this);
    }
}