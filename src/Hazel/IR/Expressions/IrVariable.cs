namespace Hazel.IR.Expressions;

public sealed class IrVariable : IrExpression
{
    public string Name
    {
        get;
    }

    public IrVariable(string name)
    {
        Name = name;
    }
}