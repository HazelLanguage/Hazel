using System.Text;

namespace Hazel.Runtime.Exceptions;

public sealed class BoundedStringOverflowException
    : IRuntimeException
{
    public void EmitCSharpRuntime(StringBuilder builder)
    {
        builder.AppendLine("""
namespace Hazel.Runtime.Exceptions
{
    public sealed class BoundedStringOverflowException
        : System.Exception
    {
        public int SourceLength { get; }

        public int MaximumLength { get; }

        public BoundedStringOverflowException(
            int sourceLength,
            int maximumLength)
            : base(
                $"Cannot convert bounded string of length {sourceLength} " +
                $"to a bounded string with maximum length {maximumLength}.")
        {
            SourceLength = sourceLength;
            MaximumLength = maximumLength;
        }
    }
}
""");
    }
}