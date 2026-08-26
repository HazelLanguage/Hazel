using Hazel.StandardLibrary.BoundedStrings;

namespace Hazel.StandardLibrary;

public sealed class StandardLibraryRegistry
    : IStandardLibraryRegistry
{
    private readonly Dictionary<
        string,
        IStandardLibraryModule> _modules;

    public StandardLibraryRegistry()
    {
        _modules = new()
        {
            [BoundedStringModule.ModuleName] =
                new BoundedStringModule()
        };
    }

    public bool TryGet(
        string name,
        out IStandardLibraryModule module)
    {
        return _modules.TryGetValue(
            name,
            out module!);
    }
}