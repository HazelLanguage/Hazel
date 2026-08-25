namespace Hazel.IR;

public sealed class IrNamespace : IrNode
{
    public string Name
    {
        get;
    }
    public List<IrType> Types { get; } = new();

    public IrNamespace(string name) => Name = name;
}