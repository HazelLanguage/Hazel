using System.Text;
using Hazel.IR;

namespace Hazel.Runtime.Components;

public interface IRuntimeComponent
{
    void RegisterRequirements(
        IrProgram program);

    void EmitCSharpRuntime(
        StringBuilder builder);
}