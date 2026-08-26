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

                    public BoundedString(
                        string value,
                        int maximumLength)
                    {
                        if (maximumLength <= 0)
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
                    }

                    public int Length =>
                        _value.Length;

                    public override string ToString() =>
                        _value;

                    public static BoundedString Narrow(
                        BoundedString value,
                        int maximumLength)
                    {
                        if (maximumLength <= 0)
                        {
                            throw new System.ArgumentOutOfRangeException(
                                nameof(maximumLength));
                        }

                        if (value._value.Length > maximumLength)
                        {
                            throw new System.ArgumentException(
                                "Bounded string exceeds maximum length.",
                                nameof(maximumLength));
                        }

                        return new BoundedString(
                            value._value,
                            maximumLength);
                    }
                }
            }
            """);

        builder.AppendLine();
    }
}