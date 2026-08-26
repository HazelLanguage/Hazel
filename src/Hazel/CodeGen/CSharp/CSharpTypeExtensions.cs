namespace Hazel.CodeGen.CSharp;

public static class CSharpTypeExtensions
{
    public static string ToCSharpTypeName(string typeName)
    {
        return typeName switch
        {
            "integer8" => "sbyte",
            "uinteger8" => "byte",

            "integer16" => "short",
            "uinteger16" => "ushort",

            "integer32" => "int",
            "uinteger32" => "uint",

            "integer64" => "long",
            "uinteger64" => "ulong",

            "integer128" => "Int128",
            "uinteger128" => "UInt128",

            "character" => "char",
            "string" => "string",

            "void" => "void",
            "dynamic" => "dynamic",

            _ => typeName
        };
    }
}