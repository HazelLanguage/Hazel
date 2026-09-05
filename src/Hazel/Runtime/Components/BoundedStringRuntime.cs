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
    private readonly HashSet<(int source, int target)> _conversions = new();

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

            case IrBoundedStringConversion conversion:
                _bounds.Add(conversion.TargetMaximumLength);
                RegisterExpression(conversion.Value);

                if (conversion.Value.Type is IrBoundedStringType sourceType)
                {
                    _conversions.Add(
                        (sourceType.MaximumLength, 
                         conversion.TargetMaximumLength));
                }
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

    private void EmitBoundedString(
        StringBuilder builder,
        int maximumLength)
    {
        builder.AppendLine($$"""
namespace Hazel.Runtime
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
            fixed (char* src = value)
            fixed (char* dst = _buffer)
            {
                System.Buffer.MemoryCopy(src, dst, {{maximumLength}} * sizeof(char), value.Length * sizeof(char));
            }
        }

""");

        foreach (var (sourceSize, _) in _conversions.Where(c => c.target == maximumLength))
        {
            builder.AppendLine($$"""
        public BoundedString{{maximumLength}}(
            BoundedString{{sourceSize}} source)
        {
            _length = source.Length;
            fixed (char* dst = _buffer)
            {
                var srcSpan = source.AsSpan();
                fixed (char* src = srcSpan)
                {
                    System.Buffer.MemoryCopy(src, dst, {{maximumLength}} * sizeof(char), source.Length * sizeof(char));
                }
            }
        }

""");
        }

        builder.AppendLine($$"""
        public ReadOnlySpan<char> AsSpan()
        {
            fixed (char* ptr = _buffer)
            {
                return new ReadOnlySpan<char>(ptr, _length);
            }
        }

        public override string ToString()
        {
            fixed (char* ptr = _buffer)
            {
                return new string(ptr, 0, _length);
            }
        }
    }
}
""");
    }
}