using Hazel.Runtime.Components;

namespace Hazel.Runtime;

public sealed class RuntimeRegistry
{
    private readonly IReadOnlyList<IRuntimeComponent> _components;

    public RuntimeRegistry(
        IEnumerable<IRuntimeComponent> components)
    {
        _components =
            components.ToArray();
    }

    public IEnumerable<IRuntimeComponent> Components =>
        _components;
}