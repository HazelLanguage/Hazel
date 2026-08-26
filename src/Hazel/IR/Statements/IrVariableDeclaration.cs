using Hazel.IR;
using Hazel.IR.Types;

public sealed class IrVariableDeclaration
    : IrStatement
{
    public string Name
    {
        get;
    }

    public IrTypeReference Type
    {
        get;
    }

    public IrExpression Value
    {
        get;
    }

    public IrVariableDeclaration(
        string name,
        IrTypeReference type,
        IrExpression value)
    {
        Name = name;
        Type = type;
        Value = value;
    }
}