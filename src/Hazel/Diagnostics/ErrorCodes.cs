namespace Hazel.Diagnostics;

/// <summary>
/// Defines static error codes for the Hazel compiler (including CLI errors).
/// For runtime exceptions, see <see cref="Hazel.Runtime.Exceptions"/>.
/// </summary>
/// <remarks>
/// <para><b>Error code allocation blocks:</b></para>
/// <list type="table">
///   <listheader>
///     <term>Range</term>
///     <description>Subsystem</description>
///   </listheader>
///   <item><term><c>HZ0001</c></term><description>Internal compiler error</description></item>
///   <item><term><c>HZ0002 - HZ0099</c></term><description>CLI / IO errors</description></item>
///   <item><term><c>HZ0100 - HZ0999</c></term><description><i>Reserved for future use.</i></description></item>
///   <item><term><c>HZ1001 - HZ1999</c></term><description>Lexer errors</description></item>
///   <item><term><c>HZ2001 - HZ2999</c></term><description>Parser errors</description></item>
///   <item><term><c>HZ3001 - HZ4999</c></term><description>Semantic / type errors</description></item>
///   <item><term><c>HZ5001 - HZ5999</c></term><description>Intermediate representation errors</description></item>
///   <item><term><c>HZ6001 - HZ6999</c></term><description>Code generation errors</description></item>
///   <item><term><c>HZ7001 - HZ8999</c></term><description><i>Reserved for future use.</i></description></item>
///   <item><term><c>HZ9001 - HZ9999</c></term><description>Warning / lint codes</description></item>
/// </list>
/// <para>
/// Codes ending in <c>000</c> (e.g., <c>HZ1000</c>, <c>HZ2000</c>) are reserved for future use and should not be used for specific error codes.
/// </para>
/// </remarks>
public static class ErrorCodes
{
    // Internal compiler error (HZ0001)
    public const string InternalCompilerError = "HZ0001";

    // CLI / IO errors (HZ0002 - HZ0099)
    public const string FileNotFound = "HZ0002";

    // Lexer errors (HZ1001 - HZ1999)
    public const string UnexpectedCharacter = "HZ1001";

    // Parser errors (HZ2001 - HZ2999)

    // Semantic / Type errors (HZ3001 - HZ4999)
    public const string ReservedNamespace = "HZ3001";
    public const string InvalidTypeModifier = "HZ3002";

    // Intermediate Representation errors (HZ5001 - HZ5999)

    // Code generation errors (HZ6001 - HZ6999)

    // Warning / lint codes (HZ9001 - HZ9999)
}