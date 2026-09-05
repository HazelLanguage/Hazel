using Hazel.Runtime.Components;
using Hazel.Runtime.Exceptions;

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

    public IEnumerable<IRuntimeException> Exceptions =>
        _components
            .SelectMany(component =>
                component.GetRequiredExceptions())
            .DistinctBy(exception =>
                exception.GetType());
}