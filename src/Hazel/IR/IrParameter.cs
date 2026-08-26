using Hazel.IR.Types;

namespace Hazel.IR;

public sealed class IrParameter : IrNode
{
    public string Name
    {
        get;
    }

    public IrTypeReference Type
    {
        get;
    }

    public IrParameter(
        string name,
        IrTypeReference type)
    {
        Name = name;
        Type = type;
    }
}