using Hazel.Diagnostics;
using Hazel.Syntax;
using Hazel.Syntax.Expressions;
using Hazel.Syntax.Statements;
using Hazel.Syntax.Types;

public sealed class VariableStatement : Statement
{
    public TypeReference Type
    {
        get;
    }
    public string Name
    {
        get;
    }
    public Expression Value
    {
        get;
    }

    public VariableStatement(
        TypeReference type,
        string name,
        Expression value,
        SourceSpan span)
        : base(span)
    {
        Type = type;
        Name = name;
        Value = value;
    }

    public override T Accept<T>(
        AstVisitor<T> visitor)
    {
        return visitor.VisitVariable(this);
    }
}