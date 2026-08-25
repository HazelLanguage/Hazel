namespace Hazel.IR;

public sealed class IrParameter : IrNode
{
    public string Name
    {
        get;
    }
    public string Type
    {
        get;
    }

    public IrParameter(string name, string type)
    {
        Name = name;
        Type = type;
    }
}