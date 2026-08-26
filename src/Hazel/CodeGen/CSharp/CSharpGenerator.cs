using System.Linq;
using System.Text;
using Hazel.IR;
using Hazel.IR.Expressions;
using Hazel.IR.Statements;
using Hazel.IR.Types;
using Hazel.StandardLibrary;
using Hazel.Syntax.Declarations;

namespace Hazel.CodeGen.CSharp;

public sealed class CSharpGenerator
{
    private readonly IStandardLibraryRegistry _standardLibrary;

    public CSharpGenerator(
        IStandardLibraryRegistry standardLibrary)
    {
        _standardLibrary = standardLibrary;
    }

    public string Generate(IrProgram program)
    {
        var builder = new StringBuilder();

        builder.AppendLine("using System;");
        builder.AppendLine();

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
                builder.Append(
                    EmitExpression(variable.Value));
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
                    builder.Append(
                        EmitExpression(returnStatement.Expression));
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
                $"new Hazel.Runtime.BoundedString(" +
                $"{EmitStringLiteral(boundedString.Value)}, " +
                $"{boundedString.MaximumLength})",

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
                "Hazel.Runtime.BoundedString",

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