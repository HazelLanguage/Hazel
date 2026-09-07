using Hazel.IR.Types;
using Hazel.Syntax.Declarations;

namespace Hazel.IR;

public sealed class IrMethod : IrNode
{
    public AccessModifiers AccessModifiers
    {
        get;
    }

    public MethodModifiers MethodModifiers
    {
        get;
    }

    public string Name
    {
        get;
    }

    public IrTypeReference ReturnType
    {
        get;
    }

    public List<IrParameter> Parameters
    {
        get;
    } = new();

    public List<IrStatement> Body
    {
        get;
    } = new();

    public IrMethod(
        AccessModifiers accessModifiers,
        MethodModifiers methodModifiers,
        string name,
        IrTypeReference returnType)
    {
        AccessModifiers = accessModifiers;
        MethodModifiers = methodModifiers;
        Name = name;
        ReturnType = returnType;
    }
}