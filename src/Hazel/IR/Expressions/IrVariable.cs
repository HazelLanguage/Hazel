namespace Hazel.IR.Expressions;

using Hazel.IR.Types;

public sealed class IrVariable
    : IrExpression
{
    public string Name
    {
        get;
    }

    public override IrValueType Type
    {
        get;
    }

    public IrVariable(
        string name,
        IrValueType type)
    {
        Name = name;
        Type = type;
    }
}