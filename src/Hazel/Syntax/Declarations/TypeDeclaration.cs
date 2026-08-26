using Hazel.Diagnostics;

namespace Hazel.Syntax.Declarations;

public sealed class TypeDeclaration : Declaration
{
    public AccessModifiers AccessModifiers
    {
        get;
    }
    public TypeModifiers Modifiers
    {
        get;
    }
    public TypeKind Kind
    {
        get;
    }
    public string Name
    {
        get;
    }
    public IReadOnlyList<Declaration> Members
    {
        get;
    }

    public TypeDeclaration(
        AccessModifiers accessModifiers,
        TypeModifiers modifiers,
        TypeKind kind,
        string name,
        IReadOnlyList<Declaration> members,
        SourceSpan span)
        : base(span)
    {
        AccessModifiers = accessModifiers;
        Modifiers = modifiers;
        Kind = kind;
        Name = name;
        Members = members;
    }

    public override T Accept<T>(AstVisitor<T> visitor)
    {
        return visitor.VisitType(this);
    }
}