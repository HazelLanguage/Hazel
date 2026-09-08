using Hazel.Lexing;

namespace Hazel.Syntax.Declarations;

[Flags]
public enum FieldModifiers
{
    None = 0,
    Static = 1 << 0,
    Unpacked = 1 << 1
}

public static class FieldModifiersExtensions
{
    public static bool IsFieldModifier(this TokenKind kind)
    {
        return kind is
            TokenKind.Static or
            TokenKind.Unpacked;
    }

    public static FieldModifiers ToFieldModifier(this TokenKind kind)
    {
        return kind switch
        {
            TokenKind.Static => FieldModifiers.Static,
            TokenKind.Unpacked => FieldModifiers.Unpacked,
            _ => throw new ArgumentException(
                $"'{kind}' is not a field modifier.",
                nameof(kind))
        };
    }

    public static bool IsValid(this FieldModifiers modifiers)
    {
        return modifiers switch
        {
            FieldModifiers.None => true,
            FieldModifiers.Static => true,
            FieldModifiers.Unpacked => true,
            FieldModifiers.Static | FieldModifiers.Unpacked => true,
            _ => false
        };
    }

    public static string ToKeyword(this FieldModifiers modifiers)
    {
        if (modifiers == FieldModifiers.None)
        {
            return string.Empty;
        }

        var keywords = new List<string>();

        if (modifiers.HasFlag(FieldModifiers.Static))
        {
            keywords.Add("static");
        }

        if (modifiers.HasFlag(FieldModifiers.Unpacked))
        {
            keywords.Add("unpacked");
        }

        return string.Join(" ", keywords);
    }
}
