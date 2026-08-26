using Hazel.Lexing;

namespace Hazel.Syntax.Declarations;

[Flags]
public enum TypeModifiers
{
    None = 0,
    Sealed = 1 << 0,
    Abstract = 1 << 1
}

public static class TypeModifiersExtensions
{
    public static bool IsTypeModifier(this TokenKind kind)
    {
        return kind is
            TokenKind.Sealed or
            TokenKind.Abstract;
    }

    public static TypeModifiers ToTypeModifier(this TokenKind kind)
    {
        return kind switch
        {
            TokenKind.Sealed
                => TypeModifiers.Sealed,

            TokenKind.Abstract
                => TypeModifiers.Abstract,

            _ => throw new ArgumentException(
                $"'{kind}' is not a type modifier.",
                nameof(kind))
        };
    }

    public static bool IsValid(this TypeModifiers modifiers)
    {
        return modifiers switch
        {
            TypeModifiers.None => true,
            TypeModifiers.Sealed => true,
            TypeModifiers.Abstract => true,

            _ => false
        };
    }

    public static string ToKeyword(this TypeModifiers modifiers)
    {
        return modifiers switch
        {
            TypeModifiers.Sealed => "sealed",

            TypeModifiers.Abstract => "abstract",

            _ => throw new ArgumentOutOfRangeException(
                nameof(modifiers),
                modifiers,
                "Invalid type modifier combination.")
        };
    }
}