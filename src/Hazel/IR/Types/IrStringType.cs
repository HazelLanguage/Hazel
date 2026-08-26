namespace Hazel.IR.Types;

public sealed class IrStringType
    : IrValueType
{
    public static IrStringType Instance { get; } = new();

    private IrStringType()
    {
    }
}