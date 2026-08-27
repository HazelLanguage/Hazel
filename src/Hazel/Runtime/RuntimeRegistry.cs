using Hazel.Runtime.Components;

namespace Hazel.Runtime;

public sealed class RuntimeRegistry
{
    private readonly IReadOnlyList<IRuntimeComponent> _components;

    public RuntimeRegistry()
    {
        _components =
        [
            new BoundedStringRuntime()
        ];
    }

    public IEnumerable<IRuntimeComponent> Components =>
        _components;
}