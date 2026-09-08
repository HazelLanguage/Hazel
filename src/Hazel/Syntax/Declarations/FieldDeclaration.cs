using Hazel.Diagnostics;
using Hazel.Syntax.Expressions;
using Hazel.Syntax.Types;

namespace Hazel.Syntax.Declarations;

public sealed class FieldDeclaration : Declaration
{
    public AccessModifiers AccessModifiers
    {
        get;
    }

    public TypeReference Type
    {
        get;
    }

    public string Name
    {
        get;
    }

    public Expression? Value
    {
        get;
    }

    public FieldDeclaration(
        AccessModifiers accessModifiers,
        TypeReference type,
        string name,
        Expression? value,
        SourceSpan span)
        : base(span)
    {
        AccessModifiers = accessModifiers;
        Type = type;
        Name = name;
        Value = value;
    }

    public override T Accept<T>(AstVisitor<T> visitor)
    {
        return visitor.VisitField(this);
    }
}
