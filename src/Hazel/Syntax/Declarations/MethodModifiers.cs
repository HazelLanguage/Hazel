namespace Hazel.Syntax.Declarations;

[Flags]
public enum MethodModifiers
{
    None = 0,
    Static = 1 << 0
}

public static class MethodModifiersExtensions
{
    public static string ToKeyword(this MethodModifiers modifiers)
    {
        return modifiers switch
        {
            MethodModifiers.None => string.Empty,
            MethodModifiers.Static => "static",
            _ => throw new ArgumentOutOfRangeException(
                nameof(modifiers),
                modifiers,
                "Invalid method modifier combination.")
        };
    }
}