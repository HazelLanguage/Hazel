# Hazel

![Hazel Logo](https://raw.githubusercontent.com/HazelLanguage/Hazel/main/assets/Hazel_Logo.webp)

A modern, high performance programming language designed for building scalable, highly type-safe, and enterprise-grade applications.

## 🌰 The Philosophy of Hazel

Hazel is built on three core engineering principles:

1. **Explicit Intent over Hidden Coercion**: Intent should be explicitly declared. Avoiding implicit type conversions, implicit variable inference, or implicit access control ensures clarity and predictability in code behavior.
2. **Readability over Conciseness**: Code is read far more times than it is written. Syntax should be clear and expressive, making codebases easier to audit and maintain, even if it requires more verbosity.
3. **Strict Type Safety over Runtime Flexibility**: Catching bugs at compile time eliminates entire classes of runtime errors. Strict type safety leads to robust systems where code behaves as expected.

## 📦 Installation

Install the compiler and CLI tool from NuGet:

```powershell
dotnet tool install -g Hazel
```

or build and install from source:

```powershell
dotnet pack
dotnet tool install -g --source ./src/Hazel/bin/Release Hazel
```

Download syntax highlighting for Visual Studio from the [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=AlinaWan.hzsh0001).

## 📖 Usage

### File/Directory

```powershell
hazel Program.hz
```

### Inline

```powershell
hazel -c "
namespace Hazel
{
    internal sealed class Program
    {
        public variable uinteger3 version = 2;
        public variable uinteger3 type = 1;
        public variable uinteger1 encrypted = 0;
        public variable uinteger1 compressed = 0;

        internal static void Main()
        {
            print \`"Hello, Hazel!\`";
        }

        private protected integer32 Add(integer32 a, integer32 b)
        {
            variable integer32 sum = a + b;
            return sum;
        }

        public string[32] ProcessUsername(string[16] rawInput)
        {
            variable string[32] cleanName = (string[32])rawInput;
            return cleanName;
        }
    }
}
"
```

### Transpiling

By default, Hazel compiles and executes the generated program. Use `-t` or `--transpile` to output the generated target source to stdout without executing:

```powershell
hazel Program.hz -t
```

### ⚙️ Development

If you are actively contributing to the compiler or want to test local source changes without reinstalling the global tool, you can run the compiler project directly:

```powershell
dotnet run --project src/Hazel -- ...
```

## 📐 Naming Conventions

Hazel follows standard .NET naming conventions:

* **PascalCase**: Namespaces, types (classes, structs), methods, and properties.
* **camelCase**: Local variables, parameters, and private fields.

## 🔣 Core Semantics

* Hazel Standard Library: The standard library wraps complex functionality in a simple, easy-to-use API.
* Explicit Variable Types: All variable assignments start with the `variable` keyword, and all variable types are required to be explicitly declared.
* UTF-16 String Encoding: Strings in Hazel are UTF-16 encoded for zero-overhead interoperability with the .NET CoreCLR.

### Explicitly Sized Integers

Integer and unsigned integer types must explicitly declare their exact bit size rather than using a generic integer keyword. Hazel provides signed and unsigned variants across five fixed bit widths:

| Signed Type  | Unsigned Type | Bit Width | Signed Range                    | Unsigned Range     | Count         |
| :----------- | :------------ | :-------- | :------------------------------ | :----------------- | :------------ |
| `integer8`   | `uinteger8`   | 8-bit     | -128 to 127                     | 0 to 255           | 256           |
| `integer16`  | `uinteger16`  | 16-bit    | -32,768 to 32,767               | 0 to 65,535        | 65,536        |
| `integer32`  | `uinteger32`  | 32-bit    | -2,147,483,648 to 2,147,483,647 | 0 to 4,294,967,295 | 4,294,967,296 |
| `integer64`  | `uinteger64`  | 64-bit    | -2⁶³ to 2⁶³ - 1                 | 0 to 2⁶⁴ - 1       | 2⁶⁴           |
| `integer128` | `uinteger128` | 128-bit   | -2¹²⁷ to 2¹²⁷ - 1               | 0 to 2¹²⁸ - 1      | 2¹²⁸          |

### Sub-Byte Integer Types

Hazel also provides signed and unsigned integer types with bit widths smaller than a byte. These types are useful when a value has a naturally constrained range and dense storage is desirable.

Sub-byte integers are available from 1 through 7 bits:

| Signed Type | Unsigned Type | Bit Width | Signed Range | Unsigned Range | Count |
| :---------- | :------------ | :-------- | :----------- | :------------- | ----- |
| `integer1`  | `uinteger1`   | 1-bit     | -1 to 0      | 0 to 1         | 2     |
| `integer2`  | `uinteger2`   | 2-bit     | -2 to 1      | 0 to 3         | 4     |
| `integer3`  | `uinteger3`   | 3-bit     | -4 to 3      | 0 to 7         | 8     |
| `integer4`  | `uinteger4`   | 4-bit     | -8 to 7      | 0 to 15        | 16    |
| `integer5`  | `uinteger5`   | 5-bit     | -16 to 15    | 0 to 31        | 32    |
| `integer6`  | `uinteger6`   | 6-bit     | -32 to 31    | 0 to 63        | 64    |
| `integer7`  | `uinteger7`   | 7-bit     | -64 to 63    | 0 to 127       | 128   |

Unlike conventional integer types, sub-byte integers are **bit-packed when used as fields within a type**. Multiple sub-byte fields can share the same underlying storage unit rather than each occupying a complete byte.

Packing applies to fields that are part of an aggregate type, such as classes, structs, and records. Sub-byte integers used as local variables, parameters, or return values retain their semantic bit width but are not required to be physically bit-packed.

For example:

```hazel
public class Packet
{
    public variable uinteger3 a;
    public variable uinteger3 b;
    public variable uinteger1 c;
    public variable uinteger1 d;
}
```

These four fields require only one 8-bit storage unit:

```text
┌───────┬───────┬───────┬───────┐
│   c   │   d   │   a   │   b   │
│   1   │   1   │   3   │   3   │
└───────┴───────┴───────┴───────┘
```

Rather than allocating a separate byte for each field, Hazel packs the fields into a shared storage unit.

Sub-byte integer types retain their exact Hazel type semantics regardless of their physical representation. Values are range-checked according to their declared bit width:

```hazel
variable uinteger3 value = 7; // ✅ Valid: 7 is within the range of uinteger3 (0 to 7).
variable uinteger3 invalid = 8; // ❌ Compilation Error: Integer literal '8' is out of range for 'uinteger3'.
```

Local variables are not required to be physically packed together:

```hazel
public uinteger4 Calculate()
{
    variable uinteger4 a = 5;
    variable uinteger4 b = 6;
    return a + b;
}
```

Here, `a` and `b` retain the `uinteger4` Hazel type, but Hazel does not require them to share a physical storage unit. The compiler and target platform may instead use a representation that is more efficient for computation, such as registers.

It should be noted that packing is a **storage representation detail of aggregate fields**, rather than a requirement that every sub-byte value physically occupy its exact number of bits. Hazel generates the required storage operations automatically, so developers do not need to manually perform bit masking or shifting when accessing packed fields.

Signed sub-byte integers use two's-complement representation. For example, `integer3` can represent values from `-4` through `3`.

Sub-byte integers are particularly useful for:

* Bit flags and boolean state
* Small enumerations
* Protocol and packet headers
* Compact metadata
* Image and color formats
* Large collections of values with small ranges
* Memory-sensitive data structures

### Namespace Reservation Rules

To avoid conflicts with the Hazel engine, the `Hazel.Runtime` namespace root and all sub-namespaces (such as `Hazel.Runtime.Foo` or `Hazel.Runtime.Exceptions`) are strictly reserved for the Hazel runtime.

Declaring user types or namespaces inside `Hazel.Runtime` or any child namespace will trigger a compilation error:

```hazel
// ❌ Compilation Error: Namespace 'Hazel.Runtime.Foo' is reserved for the Hazel runtime.
namespace Hazel.Runtime.Foo
{
    public class CustomHandler { }
}
```

#### Allowed Variations

Namespaces that merely start with "Hazel.Runtime" without matching the reserved root segment or being sub-namespaces of the reserved root are completely valid:

* Reserved: `Hazel.Runtime`, `Hazel.Runtime.*` (e.g., `Hazel.Runtime.Exceptions`, `Hazel.Runtime.Collections`)
* Allowed: `Hazel.RuntimeExtensions`, `Hazel.MyProgram`, `Hazel.RuntimeHelpers.App`

### Explicit Access Modifiers

Every namespace, type, method, field, and property must explicitly declare access control.

#### Base Modifiers

Hazel provides four foundational access keywords:

| Modifier | Access Scope |
| :--- | :--- |
| `public` | Unrestricted access across all referencing assemblies. |
| `internal` | Restricted to the declaring assembly. |
| `protected` | Restricted to the declaring class and derived types. |
| `private` | Restricted strictly to the declaring type. |

#### Combinations & Flexible Order

Access modifiers can be combined to form composite scopes. Combining keywords works in any order (`protected internal` and `internal protected` are functionally identical):

* **`protected internal`**: Accessible within the declaring assembly **OR** by derived types in other assemblies.
* **`private protected`**: Accessible within the declaring class **AND** by derived types, but **ONLY** within the same assembly.

```hazel
// ❌ Compilation Error: Missing mandatory access modifier.
class Calculator
{
    integer32 Add(integer32 a, integer32 b) { return a + b; }
}

// ✅ Correct: Explicitly declared access control modifier
internal class Calculator
{
    private integer32 Add(integer32 a, integer32 b) { return a + b; }
}
```

### First-Class Bounded Strings

Strings can carry explicit length constraints as part of their type. These constraints are checked statically whenever possible for safety, and deferred to runtime when values are dynamic, giving both static guarantees and runtime flexibility compared to standard C# strings. A variable of type `string` without square brackets is considered unbounded, while a variable of type `string[n]` is considered bounded to a maximum of `n` characters.

Bounded strings allocate their entire fixed capacity as a single, contiguous block of memory which live directly on the call stack when declared locally, or inline when embedded within other structures, classes, or arrays. For example, rather than pointing to a separate managed heap allocation, a `string[32]` reserves its full 32-character buffer upfront (64 bytes for the buffer plus 4 bytes for the length).

```hazel
variable string[32] username = "Alice";
```

#### Compile-Time Validation

String literals assigned to bounded strings are checked statically during compilation. If a string literal exceeds the target variable's maximum capacity, the compiler generates a static error:

```hazel
// ❌ Compilation Error: String literal is 41 characters long, but 'foo' has a maximum length of 32.
variable string[32] foo = "This string exceeds thirty-two characters";
```

#### Type Conversions

Bounded strings of different lengths are distinct types. There is no implicit conversion between bounded string types, even when assigning a smaller bounded string to a larger bounded string.

For instance, returning a `string[16]` from a method expecting a `string[32]` will trigger a compiler error:

```hazel
// ❌ Compilation Error: Cannot return value of type 'string[16]' from method returning 'string[32]'.
public string[32] GetName(string[16] input)
{
    return input; 
}
```

To convert between bounded string types, you must use an explicit cast:

```hazel
// ✅ Correct: Explicitly cast to the target bounded size
public string[32] GetName(string[16] input)
{
    return (string[32])input; 
}
```

When casting between bounded string types, widening conversions (e.g., `string[16]` to `string[32]`) are always safe. However, attempting to narrow a string when the source string's actual length exceeds the target capacity will throw a `BoundedStringOverflowException` at runtime:

```hazel
variable string[64] longName = "This string exceeds thirty-two characters";

// ❌ Runtime Error: Hazel.Runtime.Exceptions.BoundedStringOverflowException: Cannot convert bounded string of length 41 to a bounded string with maximum length 32.
variable string[32] shortName = (string[32])longName;
```

Casting between bounded string types performs a direct, zero-allocation memory copy from the source buffer into the target buffer. Because bounded strings are stack-allocated value types, this operation copies raw memory blocks directly without instantiating heap objects or invoking string serialization routines.

## 🧩 Contributing

We welcome contributions from the community! Tests and TextMate grammars are highly needed.

## 📄 License

The Hazel compiler is open-source and licensed under the [MPL-2.0 License](LICENSE).

---

Copyright © 2026 Hazel Foundation  
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀🌰