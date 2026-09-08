using Hazel.IR.Types;
using Hazel.Syntax.Declarations;

namespace Hazel.IR;

public sealed class IrField : IrNode
{
    public AccessModifiers AccessModifiers
    {
        get;
    }

    public string Name
    {
        get;
    }

    public IrTypeReference Type
    {
        get;
    }

    public IrExpression? Value
    {
        get;
    }

    public IrField(
        AccessModifiers accessModifiers,
        string name,
        IrTypeReference type,
        IrExpression? value)
    {
        AccessModifiers = accessModifiers;
        Name = name;
        Type = type;
        Value = value;
    }
}
