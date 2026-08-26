namespace Hazel.StandardLibrary;

public interface IStandardLibraryRegistry
{
    bool TryGet(
        string name,
        out IStandardLibraryModule module);
}