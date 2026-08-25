namespace Hazel.IR.Statements;

public sealed class IrVariableDeclaration
    : IrStatement
{
    public string Name
    {
        get;
    }
    public IrExpression Value
    {
        get;
    }

    public IrVariableDeclaration(
        string name,
        IrExpression value)
    {
        Name = name;
        Value = value;
    }
}