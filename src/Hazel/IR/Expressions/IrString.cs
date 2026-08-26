namespace Hazel.IR.Expressions;

using Hazel.IR.Types;

public sealed class IrString
    : IrExpression
{
    public string Value
    {
        get;
    }

    public override IrValueType Type =>
        IrStringType.Instance;

    public IrString(string value)
    {
        Value = value;
    }
}