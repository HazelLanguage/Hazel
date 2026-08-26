using System.Text;

namespace Hazel.StandardLibrary.BoundedStrings;

public sealed class BoundedStringModule
    : IStandardLibraryModule
{
    public const string ModuleName =
        "Hazel.Strings.Bounded";

    public string Name =>
        ModuleName;

    public void EmitCSharpRuntime(
        StringBuilder builder)
    {
        builder.AppendLine("""
            namespace Hazel.Runtime
            {
                public readonly struct BoundedString
                {
                    private readonly string _value;
                    private readonly int _maximumLength;

                    public BoundedString(
                        string value,
                        int maximumLength)
                    {
                        if (maximumLength < 0)
                        {
                            throw new System.ArgumentOutOfRangeException(
                                nameof(maximumLength));
                        }

                        if (value.Length > maximumLength)
                        {
                            throw new System.ArgumentException(
                                "Bounded string exceeds maximum length.",
                                nameof(value));
                        }

                        _value = value;
                        _maximumLength = maximumLength;
                    }

                    public int Length =>
                        _value.Length;

                    public int MaximumLength =>
                        _maximumLength;

                    public override string ToString() =>
                        _value;
                }
            }
            """);

        builder.AppendLine();
    }
}