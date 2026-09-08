using System.Linq;
using System.Text;
using Hazel.IR;
using Hazel.IR.Expressions;
using Hazel.IR.Statements;
using Hazel.IR.Types;
using Hazel.Runtime;
using Hazel.Runtime.Components;
using Hazel.Runtime.Exceptions;
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

        builder.AppendLine("using System;");

        foreach (IRuntimeException exception in _runtime.Exceptions)
        {
            exception.EmitCSharpRuntime(builder);
        }

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

                var instancePackedFields = type.Fields
                    .Where(field =>
                        !field.Modifiers.HasFlag(FieldModifiers.Static) &&
                        !field.Modifiers.HasFlag(FieldModifiers.Unpacked) &&
                        TryGetIntegerType(field.Type, out var integerType) &&
                        integerType.BitWidth < 8)
                    .ToList();

                var staticPackedFields = type.Fields
                    .Where(field =>
                        field.Modifiers.HasFlag(FieldModifiers.Static) &&
                        !field.Modifiers.HasFlag(FieldModifiers.Unpacked) &&
                        TryGetIntegerType(field.Type, out var integerType) &&
                        integerType.BitWidth < 8)
                    .ToList();

                foreach (var packedFieldSet in new[]
                {
                    (IsStatic: false, Fields: instancePackedFields),
                    (IsStatic: true, Fields: staticPackedFields)
                })
                {
                    if (packedFieldSet.Fields.Count == 0)
                    {
                        continue;
                    }

                    var layout = new PackedStorageLayout();

                    foreach (var field in packedFieldSet.Fields)
                    {
                        TryGetIntegerType(field.Type, out var integerType);
                        layout.AddField(field.Name, integerType);
                    }

                    var initialStorage = new byte[layout.StorageUnitCount];

                    foreach (var field in packedFieldSet.Fields)
                    {
                        if (field.Value == null)
                            continue;

                        var packedField =
                            layout.Fields.Single(f => f.Name == field.Name);

                        if (field.Value is not IrConstant constant)
                        {
                            throw new NotImplementedException(
                                $"Packed field '{field.Name}' has a non-constant initializer, " +
                                "which is not currently supported.");
                        }

                        int value = int.Parse(constant.Value.ToString());

                        initialStorage[packedField.StorageUnitIndex] |=
                            (byte)(
                                (value & ((1 << packedField.BitWidth) - 1))
                                << packedField.StorageUnitBitOffset);
                    }

                    string storagePrefix = packedFieldSet.IsStatic ? "_staticStorage" : "_storage";

                    for (int i = 0; i < layout.StorageUnitCount; i++)
                    {
                        builder.Append("        private ");
                        if (packedFieldSet.IsStatic)
                        {
                            builder.Append("static ");
                        }

                        builder.Append(PackedStorageLayout.GetStorageUnitTypeName(layout.StorageUnitBits));
                        builder.Append(" ");
                        builder.Append(storagePrefix);
                        builder.Append(i);

                        if (initialStorage[i] != 0)
                        {
                            builder.Append(" = 0b");
                            builder.Append(Convert.ToString(initialStorage[i], 2).PadLeft(8, '0'));
                        }

                        builder.AppendLine(";");
                    }

                    foreach (var field in packedFieldSet.Fields)
                    {
                        TryGetIntegerType(field.Type, out var integerType);
                        var packedField = layout.Fields.Single(f => f.Name == field.Name);
                        string propertyType = EmitType(field.Type);
                        string fieldAccess = field.AccessModifiers.ToKeyword();
                        string staticModifier = packedFieldSet.IsStatic ? "static " : string.Empty;
                        string storageName = $"{storagePrefix}{packedField.StorageUnitIndex}";

                        builder.Append("        ");
                        if (!string.IsNullOrEmpty(fieldAccess))
                        {
                            builder.Append(fieldAccess);
                            builder.Append(" ");
                        }

                        if (!string.IsNullOrEmpty(staticModifier))
                        {
                            builder.Append(staticModifier);
                        }

                        builder.Append(propertyType);
                        builder.Append(" ");
                        builder.Append(field.Name);
                        builder.AppendLine();

                        builder.AppendLine("        {");
                        builder.Append("            get => ");
                        builder.Append(EmitPackedFieldGetter(packedField, propertyType, storageName));
                        builder.AppendLine(";");
                        builder.AppendLine("            set");
                        builder.AppendLine("            {");
                        builder.Append("                ");
                        builder.Append(EmitPackedFieldSetter(packedField, storageName));
                        builder.AppendLine(";");
                        builder.AppendLine("            }");
                        builder.AppendLine("        }");
                    }
                }

                foreach (var field in type.Fields.Where(field =>
                    !TryGetIntegerType(field.Type, out var integerType) ||
                    integerType.BitWidth >= 8 ||
                    field.Modifiers.HasFlag(FieldModifiers.Unpacked)))
                {
                    builder.Append("        ");
                    builder.Append(field.AccessModifiers.ToKeyword());
                    if (!string.IsNullOrEmpty(field.AccessModifiers.ToKeyword()))
                    {
                        builder.Append(" ");
                    }

                    if (field.Modifiers.HasFlag(FieldModifiers.Static))
                    {
                        builder.Append("static ");
                    }

                    builder.Append(EmitType(field.Type));
                    builder.Append(" ");
                    builder.Append(field.Name);

                    if (field.Value != null)
                    {
                        builder.Append(" = ");
                        builder.Append(EmitExpression(field.Value));
                    }

                    builder.AppendLine(";");
                }

                foreach (var method in type.Methods)
                {
                    _currentReturnType = method.ReturnType;

                    builder.Append("        ");
                    builder.Append(method.AccessModifiers.ToKeyword());

                    if (!string.IsNullOrEmpty(method.AccessModifiers.ToKeyword()))
                    {
                        builder.Append(" ");
                    }

                    string methodModifierKeyword = method.MethodModifiers.ToKeyword();
                    if (!string.IsNullOrEmpty(methodModifierKeyword))
                    {
                        builder.Append(methodModifierKeyword);
                        builder.Append(" ");
                    }

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

            case IrPrintStatement printStatement:

                builder.Append("            ");
                builder.Append("System.Console.Write(");
                builder.Append(EmitExpression(printStatement.Expression));
                builder.AppendLine(");");

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

    private static bool TryGetIntegerType(
        IrTypeReference type,
        out IrIntegerType integerType)
    {
        if (type is not IrNamedType named)
        {
            integerType = null!;
            return false;
        }

        string name = named.Name;

        if (name.StartsWith("uinteger", StringComparison.Ordinal))
        {
            if (int.TryParse(name["uinteger".Length..], out int bits))
            {
                integerType = new IrIntegerType(bits, false);
                return true;
            }
        }

        if (name.StartsWith("integer", StringComparison.Ordinal))
        {
            if (int.TryParse(name["integer".Length..], out int bits))
            {
                integerType = new IrIntegerType(bits, true);
                return true;
            }
        }

        integerType = null!;
        return false;
    }

    private static bool TryGetIntegerType(
        IrValueType type,
        out IrIntegerType integerType)
    {
        if (type is IrIntegerType irInteger)
        {
            integerType = irInteger;
            return true;
        }

        integerType = null!;
        return false;
    }

    private static string EmitValueType(
        IrValueType type)
    {
        return type switch
        {
            IrIntegerType integer =>
                CSharpTypeExtensions.ToCSharpTypeName(
                    integer.IsSigned
                        ? $"integer{integer.BitWidth}"
                        : $"uinteger{integer.BitWidth}"),

            IrStringType => "string",

            IrBoundedStringType bounded =>
                $"Hazel.Runtime.BoundedString{bounded.MaximumLength}",

            _ => throw new Exception(
                $"Unknown IR value type: {type.GetType().Name}")
        };
    }

    private static string EmitPackedFieldGetter(
        PackedField field,
        string propertyType,
        string storageName)
    {
        string unitName =
            storageName;

        int mask =
            (1 << field.BitWidth) - 1;

        string raw =
            $"(({unitName} >> {field.StorageUnitBitOffset}) & {mask})";

        if (!field.Type.IsSigned)
        {
            return $"({propertyType}){raw}";
        }

        int signBit =
            1 << (field.BitWidth - 1);

        return
            $"({propertyType})" +
            $"(({raw} & {signBit}) != 0 " +
            $"? {raw} - {1 << field.BitWidth} " +
            $": {raw})";
    }

    private static string EmitPackedFieldSetter(
        PackedField field,
        string storageName)
    {
        string unitType =
            PackedStorageLayout.GetStorageUnitTypeName(
                field.StorageUnitBits);

        string unitName =
            storageName;

        int valueMask =
            (1 << field.BitWidth) - 1;

        int storageMask =
            valueMask << field.StorageUnitBitOffset;

        return
            $"{unitName} = ({unitType})" +
            $"(({unitName} & ~{storageMask}) | " +
            $"((value & {valueMask}) << " +
            $"{field.StorageUnitBitOffset}))";
    }

    private static string GetStorageUnitMaskExpression(
        string unitType,
        int bitIndex,
        bool isClear)
    {
        string literal = unitType switch
        {
            "byte" => $"((byte)(1 << {bitIndex}))",
            "ushort" => $"((ushort)(1 << {bitIndex}))",
            "uint" => $"((uint)(1 << {bitIndex}))",
            "ulong" => $"((ulong)(1 << {bitIndex}))",
            "System.UInt128" => $"((System.UInt128)1 << {bitIndex})",
            _ => throw new InvalidOperationException($"Unsupported storage unit type: {unitType}.")
        };

        return isClear ? $"(~{literal})" : literal;
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
                TryGetIntegerType(binary.Type, out var integerType)
                    ? $"({EmitValueType(integerType)})({EmitExpression(binary.Left)} " +
                      $"{binary.Operator} " +
                      $"{EmitExpression(binary.Right)})"
                    : $"({EmitExpression(binary.Left)} " +
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