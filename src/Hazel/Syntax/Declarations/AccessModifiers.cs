using Hazel.Lexing;

namespace Hazel.Syntax.Declarations;

[Flags]
public enum AccessModifiers
{
    None = 0,
    Public = 1 << 0,
    Private = 1 << 1,
    Protected = 1 << 2,
    Internal = 1 << 3
}

public static class AccessModifiersExtensions
{
    public static bool IsAccessModifier(this TokenKind kind)
    {
        return kind is
            TokenKind.Public or
            TokenKind.Private or
            TokenKind.Protected or
            TokenKind.Internal;
    }

    public static AccessModifiers ToAccessModifier(
        this TokenKind kind)
    {
        return kind switch
        {
            TokenKind.Public =>
                AccessModifiers.Public,

            TokenKind.Private =>
                AccessModifiers.Private,

            TokenKind.Protected =>
                AccessModifiers.Protected,

            TokenKind.Internal =>
                AccessModifiers.Internal,

            _ => throw new ArgumentException(
                $"'{kind}' is not an access modifier.",
                nameof(kind))
        };
    }

    public static bool IsValid(
        this AccessModifiers modifiers)
    {
        return modifiers switch
        {
            AccessModifiers.Public => true,
            AccessModifiers.Private => true,
            AccessModifiers.Protected => true,
            AccessModifiers.Internal => true,

            AccessModifiers.Protected |
            AccessModifiers.Internal => true,

            AccessModifiers.Private |
            AccessModifiers.Protected => true,

            _ => false
        };
    }

    public static string ToKeyword(
        this AccessModifiers modifiers)
    {
        return modifiers switch
        {
            AccessModifiers.Public =>
                "public",

            AccessModifiers.Private =>
                "private",

            AccessModifiers.Protected =>
                "protected",

            AccessModifiers.Internal =>
                "internal",

            AccessModifiers.Protected |
            AccessModifiers.Internal =>
                "protected internal",

            AccessModifiers.Private |
            AccessModifiers.Protected =>
                "private protected",

            _ => throw new ArgumentOutOfRangeException(
                nameof(modifiers),
                modifiers,
                "Invalid access modifier combination.")
        };
    }
}