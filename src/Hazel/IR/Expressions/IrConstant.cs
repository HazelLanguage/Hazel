using Hazel.IR.Types;

namespace Hazel.IR.Expressions;

public sealed class IrConstant
    : IrExpression
{
    public long Value
    {
        get;
    }

    public override IrValueType Type =>
        new IrIntegerType(32, true);

    public IrConstant(long value)
    {
        Value = value;
    }
}