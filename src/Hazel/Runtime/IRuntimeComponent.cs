using System.Text;

namespace Hazel.Runtime;

public interface IRuntimeComponent
{
    void EmitCSharpRuntime(
        StringBuilder builder);
}