namespace Hazel.Diagnostics;

public static class ErrorCodes
{
    // CLI / IO errors (HZ0000 - HZ0999)
    public const string InternalCompilerError = "HZ0001";
    public const string FileNotFound = "HZ0002";

    // Lexer errors (HZ1000 - HZ1999)
    public const string UnexpectedCharacter = "HZ1001";

    // Parser errors (HZ2000 - HZ2999)

    // Semantic / Type errors (HZ3000 - HZ3999)
}