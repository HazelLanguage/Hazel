namespace Hazel.IR;

using Hazel.IR.Types;

public abstract class IrExpression : IrNode
{
    public abstract IrValueType Type
    {
        get;
    }
}