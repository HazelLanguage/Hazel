namespace Hazel.IR.Expressions;

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