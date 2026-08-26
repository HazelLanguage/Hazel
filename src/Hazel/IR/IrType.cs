using Hazel.Syntax;
using Hazel.Syntax.Declarations;

namespace Hazel.IR;

public sealed class IrType : IrNode
{
    public AccessModifiers AccessModifiers
    {
        get;
    }
    public TypeModifiers Modifiers
    {
        get;
    }
    public string Name
    {
        get;
    }
    public TypeKind Kind
    {
        get;
    }
    public List<IrMethod> Methods { get; } = new();

    public IrType(
        AccessModifiers accessModifiers,
        TypeModifiers modifiers,
        string name,
        TypeKind kind)
    {
        AccessModifiers = accessModifiers;
        Modifiers = modifiers;
        Name = name;
        Kind = kind;
    }
}