using System.Text;

namespace Hazel.StandardLibrary;

public interface IStandardLibraryModule
{
    string Name
    {
        get;
    }

    void EmitCSharpRuntime(
        StringBuilder builder);
}