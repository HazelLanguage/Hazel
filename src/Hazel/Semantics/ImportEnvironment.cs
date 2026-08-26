namespace Hazel.Semantics;

public sealed class ImportEnvironment
{
    private readonly HashSet<string> _imports =
        new();

    public void Add(string name)
    {
        _imports.Add(name);
    }

    public bool IsImported(string name)
    {
        return _imports.Contains(name);
    }
}