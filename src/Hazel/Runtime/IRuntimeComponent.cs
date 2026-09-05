using System.Text;
using Hazel.IR;
using Hazel.Runtime.Exceptions;

namespace Hazel.Runtime.Components;

public interface IRuntimeComponent
{
    void RegisterRequirements(
        IrProgram program);

    IEnumerable<IRuntimeException> GetRequiredExceptions();

    void EmitCSharpRuntime(
        StringBuilder builder);
}