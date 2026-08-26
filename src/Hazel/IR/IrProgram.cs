namespace Hazel.IR;

public sealed class IrProgram : IrNode
{
    public List<IrNamespace> Namespaces { get; } = new();

    public HashSet<string> ImportedLibraries { get; } = new();
}