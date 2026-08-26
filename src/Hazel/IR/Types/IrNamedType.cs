namespace Hazel.IR.Types;

public sealed class IrNamedType
    : IrTypeReference
{
    public string Name
    {
        get;
    }

    public IrNamedType(
        string name)
    {
        Name = name;
    }
}