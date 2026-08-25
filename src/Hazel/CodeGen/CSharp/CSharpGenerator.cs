using System.Linq;
using System.Text;
using Hazel.IR;
using Hazel.IR.Expressions;
using Hazel.IR.Statements;
using Hazel.Syntax.Declarations;

namespace Hazel.CodeGen.CSharp;

public sealed class CSharpGenerator
{
    public string Generate(IrProgram program)
    {
        var builder = new StringBuilder();

        builder.AppendLine("using System;");
        builder.AppendLine();

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
                builder.Append(type.AccessModifiers.ToKeyword());
                builder.Append(" ");
                builder.Append(keyword);
                builder.Append(" ");
                builder.AppendLine(type.Name);
                builder.AppendLine("    {");

                foreach (var method in type.Methods)
                {
                    builder.Append("        ");
                    builder.Append(method.AccessModifiers.ToKeyword());
                    builder.Append(" ");
                    builder.Append(method.ReturnType);
                    builder.Append(" ");
                    builder.Append(method.Name);
                    builder.Append("(");

                    builder.Append(string.Join(", ", method.Parameters.Select(p => $"{p.Type} {p.Name}")));

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

                builder.Append("            var ");
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

            _ => throw new Exception(
                $"Unknown IR expression: " +
                expression.GetType().Name)
        };
    }
}