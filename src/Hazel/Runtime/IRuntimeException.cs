using System.Text;

namespace Hazel.Runtime.Exceptions;

public interface IRuntimeException
{
    void EmitCSharpRuntime(
        StringBuilder builder);
}