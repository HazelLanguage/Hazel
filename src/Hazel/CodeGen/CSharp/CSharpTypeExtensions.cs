namespace Hazel.CodeGen.CSharp;

public static class CSharpTypeExtensions
{
    public static string ToCSharpTypeName(string typeName)
    {
        if (typeName.StartsWith("integer", StringComparison.Ordinal))
        {
            return ToCSharpIntegerTypeName(typeName, isSigned: true);
        }

        if (typeName.StartsWith("uinteger", StringComparison.Ordinal))
        {
            return ToCSharpIntegerTypeName(typeName, isSigned: false);
        }

        return typeName switch
        {
            "character" => "char",
            "string" => "string",

            "void" => "void",
            "dynamic" => "dynamic",

            _ => typeName
        };
    }

    private static string ToCSharpIntegerTypeName(
        string typeName,
        bool isSigned)
    {
        int width = ParseBitWidth(typeName);

        if (width <= 0)
        {
            return isSigned ? "int" : "uint";
        }

        if (width <= 8)
        {
            return isSigned ? "sbyte" : "byte";
        }

        if (width <= 16)
        {
            return isSigned ? "short" : "ushort";
        }

        if (width <= 32)
        {
            return isSigned ? "int" : "uint";
        }

        if (width <= 64)
        {
            return isSigned ? "long" : "ulong";
        }

        return isSigned ? "System.Int128" : "System.UInt128";
    }

    private static int ParseBitWidth(string typeName)
    {
        string suffix;

        if (typeName.StartsWith("uinteger", StringComparison.Ordinal))
        {
            suffix = typeName["uinteger".Length..];
        }
        else if (typeName.StartsWith("integer", StringComparison.Ordinal))
        {
            suffix = typeName["integer".Length..];
        }
        else
        {
            suffix = string.Empty;
        }

        if (int.TryParse(suffix, out int bitWidth))
        {
            return bitWidth;
        }

        return 0;
    }
}
