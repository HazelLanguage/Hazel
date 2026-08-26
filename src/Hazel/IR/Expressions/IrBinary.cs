namespace Hazel.IR.Expressions;

using Hazel.IR.Types;

public sealed class IrBinary : IrExpression
{
    public IrExpression Left
    {
        get;
    }

    public IrExpression Right
    {
        get;
    }

    public string Operator
    {
        get;
    }

    public override IrValueType Type =>
        Left.Type;

    public IrBinary(
        IrExpression left,
        string @operator,
        IrExpression right)
    {
        Left = left;
        Operator = @operator;
        Right = right;
    }
}