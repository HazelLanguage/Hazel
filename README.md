# Hazel

![Hazel Logo](https://raw.githubusercontent.com/HazelLanguage/Hazel/main/assets/Hazel_Logo.webp)

A modern, high performance programming language designed for building scalable, highly type-safe, and enterprise-grade applications.

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

File/Directory:

```powershell
hazel Program.hz
```

Inline:

```powershell
hazel -c "
namespace Hazel
{
    internal sealed class Calculator
    {
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

### ⚙️ Development

If you are actively contributing to the compiler or want to test local source changes without reinstalling the global tool, you can run the compiler project directly:

```powershell
dotnet run --project src/Hazel -- ...
```

## 🔣 Core Semantics

* Hazel Standard Library: The standard library wraps complex functionality in a simple, easy-to-use API.
* Mandatory Variable Types: All variable assignments start with the `variable` keyword, and all variable types are required to be explicitly declared.
* Explicitly Sized Integers: Integer and unsigned integer types must declare their exact bit size (e.g., `integer32`, `uinteger32`) rather than using a bare `integer` keyword.

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

<div align="center">
  <p>Copyright © 2026 Hazel Foundation</p>
  <p>🌰</p>
</div>