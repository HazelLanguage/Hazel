using System.Linq;
using System.Text;
using Hazel.IR;
using Hazel.IR.Expressions;
using Hazel.IR.Statements;
using Hazel.IR.Types;
using Hazel.Runtime;
using Hazel.Runtime.Components;
using Hazel.StandardLibrary;
using Hazel.Syntax.Declarations;

namespace Hazel.CodeGen.CSharp;

public sealed class CSharpGenerator
{
    private readonly RuntimeRegistry _runtime;
    private readonly IStandardLibraryRegistry _standardLibrary;
    private IrTypeReference? _currentReturnType;

    public CSharpGenerator(
        RuntimeRegistry runtime,
        IStandardLibraryRegistry standardLibrary)
    {
        _runtime = runtime;
        _standardLibrary = standardLibrary;
    }

    public string Generate(IrProgram program)
    {
        foreach (IRuntimeComponent component in _runtime.Components)
        {
            component.RegisterRequirements(program);
        }

        var builder = new StringBuilder();

        foreach (IRuntimeComponent component in _runtime.Components)
        {
            component.EmitCSharpRuntime(builder);
        }

        foreach (string libraryName in program.ImportedLibraries)
        {
            if (_standardLibrary.TryGet(
                    libraryName,
                    out IStandardLibraryModule module))
            {
                module.EmitCSharpRuntime(builder);
            }
        }

        foreach (var ns in program.Namespaces)
        {
            builder.Append("namespace ");
            builder.AppendLine(ns.Name);
            builder.AppendLine("{");

            foreach (var type in ns.Types)
            {
                string keyword = type.Kind switch
                {
                    TypeKind.Class => "class",
                    TypeKind.Struct => "struct",
                    TypeKind.Record => "record",
                    _ => "class"
                };

                builder.Append("    ");

                string access =
                    type.AccessModifiers.ToKeyword();

                string modifiers =
                    type.Modifiers != TypeModifiers.None ? type.Modifiers.ToKeyword() : string.Empty;

                if (!string.IsNullOrEmpty(access))
                {
                    builder.Append(access);
                    builder.Append(" ");
                }

                if (!string.IsNullOrEmpty(modifiers))
                {
                    builder.Append(modifiers);
                    builder.Append(" ");
                }

                builder.Append(keyword);
                builder.Append(" ");
                builder.AppendLine(type.Name);
                builder.AppendLine("    {");

                foreach (var method in type.Methods)
                {
                    _currentReturnType = method.ReturnType;

                    builder.Append("        ");
                    builder.Append(method.AccessModifiers.ToKeyword());
                    builder.Append(" ");
                    builder.Append(
                        EmitType(method.ReturnType));
                    builder.Append(" ");
                    builder.Append(method.Name);
                    builder.Append("(");

                    builder.Append(
                        string.Join(
                            ", ",
                            method.Parameters.Select(
                                p =>
                                    $"{EmitType(p.Type)} {p.Name}")));

                    builder.AppendLine(")");
                    builder.AppendLine("        {");

                    foreach (var statement in method.Body)
                    {
                        EmitStatement(builder, statement);
                    }

                    builder.AppendLine("        }");

                    _currentReturnType = null;
                }

                builder.AppendLine("    }");
            }

            builder.AppendLine("}");
        }

        return builder.ToString();
    }

    private void EmitStatement(
        StringBuilder builder,
        IrStatement statement)
    {
        switch (statement)
        {
            case IrVariableDeclaration variable:

                builder.Append("            ");
                builder.Append(EmitType(variable.Type));
                builder.Append(" ");
                builder.Append(variable.Name);
                builder.Append(" = ");
                if (variable.Type is IrBoundedStringType targetType)
                {
                    string expression =
                        EmitExpression(variable.Value);

                    if (variable.Value.Type is IrBoundedStringType sourceType)
                    {
                        if (sourceType.MaximumLength <=
                            targetType.MaximumLength)
                        {
                            builder.Append(expression);
                        }
                    }
                    else if (variable.Value.Type is IrStringType)
                    {
                        // text -> bounded<N>
                        builder.Append(
                            $"new Hazel.Runtime.BoundedString" +
                            $"{targetType.MaximumLength}(" +
                            $"{expression})");
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            $"Cannot convert {variable.Value.Type} " +
                            $"to bounded string.");
                    }
                }
                else
                {
                    builder.Append(
                        EmitExpression(variable.Value));
                }
                builder.AppendLine(";");

                break;

            case IrExpressionStatement expression:

                builder.Append("            ");
                builder.Append(
                    EmitExpression(expression.Expression));
                builder.AppendLine(";");

                break;

            case IrReturnStatement returnStatement:

                builder.Append("            return");

                if (returnStatement.Expression != null)
                {
                    builder.Append(" ");
                    if (_currentReturnType is IrBoundedStringType boundedReturnType)
                    {
                        string expression =
                            EmitExpression(returnStatement.Expression);

                        if (returnStatement.Expression.Type
                            is IrBoundedStringType sourceType)
                        {
                            if (sourceType.MaximumLength <=
                                boundedReturnType.MaximumLength)
                            {
                                builder.Append(expression);
                            }
                            else
                            {
                                builder.Append(
                                    $"Hazel.Runtime.BoundedString.Narrow(" +
                                    $"{expression}, " +
                                    $"{boundedReturnType.MaximumLength})");
                            }
                        }
                        else if (returnStatement.Expression.Type is IrStringType)
                        {
                            builder.Append(
                                $"new Hazel.Runtime.BoundedString" +
                                $"{boundedReturnType.MaximumLength}(" +
                                $"{expression})");
                        }
                        else
                        {
                            throw new InvalidOperationException(
                                $"Cannot return " +
                                $"{returnStatement.Expression.Type} " +
                                $"as bounded string.");
                        }
                    }
                    else
                    {
                        builder.Append(
                            EmitExpression(returnStatement.Expression));
                    }
                }

                builder.AppendLine(";");

                break;

            default:
                throw new Exception(
                    $"Unknown IR statement: " +
                    statement.GetType().Name);
        }
    }

    private string EmitExpression(
        IrExpression expression)
    {
        return expression switch
        {
            IrConstant constant =>
                constant.Value.ToString(),

            IrVariable variable =>
                variable.Name,

            IrBinary binary =>
                $"({EmitExpression(binary.Left)} " +
                $"{binary.Operator} " +
                $"{EmitExpression(binary.Right)})",

            IrString stringExpression =>
                EmitStringLiteral(stringExpression.Value),

            IrBoundedString boundedString =>
                $"new Hazel.Runtime.BoundedString" +
                $"{boundedString.MaximumLength}(" +
                $"{EmitStringLiteral(boundedString.Value)})",

            IrBoundedStringConversion conversion =>
                $"new Hazel.Runtime.BoundedString" +
                $"{conversion.TargetMaximumLength}(" +
                $"{EmitExpression(conversion.Value)})",

            _ => throw new Exception(
                $"Unknown IR expression: " +
                expression.GetType().Name)
        };
    }

    private string EmitType(
    IrTypeReference type)
    {
        return type switch
        {
            IrNamedType named =>
                EmitNamedType(named),

            IrBoundedStringType bounded =>
                $"Hazel.Runtime.BoundedString{bounded.MaximumLength}",

            _ => throw new Exception(
                $"Unknown IR type: {type.GetType().Name}")
        };
    }

    private string EmitNamedType(
        IrNamedType type)
    {
        return CSharpTypeExtensions.ToCSharpTypeName(type.Name);
    }

    private string EmitStringLiteral(
    string value)
    {
        return "\"" +
            value
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n")
                .Replace("\t", "\\t") +
            "\"";
    }
}