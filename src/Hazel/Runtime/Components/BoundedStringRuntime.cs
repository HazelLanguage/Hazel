using System.Text;
using Hazel.IR;
using Hazel.IR.Expressions;
using Hazel.IR.Statements;
using Hazel.IR.Types;

namespace Hazel.Runtime.Components;

public sealed class BoundedStringRuntime
    : IRuntimeComponent
{
    private readonly HashSet<int> _bounds = new();

    public void RegisterRequirements(
        IrProgram program)
    {
        foreach (var ns in program.Namespaces)
        {
            foreach (var type in ns.Types)
            {
                foreach (var method in type.Methods)
                {
                    RegisterType(method.ReturnType);

                    foreach (var parameter in method.Parameters)
                    {
                        RegisterType(parameter.Type);
                    }

                    foreach (var statement in method.Body)
                    {
                        RegisterStatement(statement);
                    }
                }
            }
        }
    }

    private void RegisterType(
        IrTypeReference type)
    {
        if (type is IrBoundedStringType bounded)
        {
            _bounds.Add(
                bounded.MaximumLength);
        }
    }

    private void RegisterExpression(
        IrExpression expression)
    {
        switch (expression)
        {
            case IrBoundedString bounded:
                _bounds.Add(
                    bounded.MaximumLength);
                break;
        }
    }

    private void RegisterStatement(
        IrStatement statement)
    {
        switch (statement)
        {
            case IrVariableDeclaration variable:
                RegisterType(variable.Type);
                RegisterExpression(variable.Value);
                break;

            case IrExpressionStatement expression:
                RegisterExpression(
                    expression.Expression);
                break;

            case IrReturnStatement returnStatement:
                if (returnStatement.Expression is not null)
                {
                    RegisterExpression(
                        returnStatement.Expression);
                }

                break;

            default:
                throw new InvalidOperationException(
                    $"Unknown IR statement: " +
                    statement.GetType().Name);
        }
    }

    public void EmitCSharpRuntime(
        StringBuilder builder)
    {
        foreach (int maximumLength in _bounds.Order())
        {
            EmitBoundedString(
                builder,
                maximumLength);
        }
    }

    private static void EmitBoundedString(
    StringBuilder builder,
    int maximumLength)
    {
        builder.AppendLine($$"""
        namespace Hazel.Runtime;
        {
            public unsafe struct BoundedString{{maximumLength}}
            {
                private fixed char _buffer[{{maximumLength}}];
                private int _length;

                public int Length =>
                    _length;

                public BoundedString{{maximumLength}}(
                    string value)
                {
                    if (value.Length > {{maximumLength}})
                    {
                        throw new System.ArgumentException(
                            "Bounded string exceeds maximum length.",
                            nameof(value));
                    }

                    _length = value.Length;

                    for (int i = 0; i < value.Length; i++)
                    {
                        _buffer[i] = value[i];
                    }
                }

                public override string ToString()
                {
                    return new string(
                        _buffer,
                        0,
                        _length);
                }
            }
        }
        """);
    }
}