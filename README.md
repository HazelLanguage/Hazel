# Hazel

![Hazel Logo](https://raw.githubusercontent.com/HazelLanguage/Hazel/main/assets/Hazel_Logo.webp)

A modern, high performance programming language designed for building scalable, type-safe, and enterprise-grade applications.

## 📦 Installation

Install the CLI tool from NuGet:

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
import Hazel.Strings.Bounded;

namespace Hazel
{
    internal sealed class Calculator
    {
        private protected integer32 Add(integer32 a, integer32 b)
        {
            variable integer32 sum = a + b;
            return sum;
        }

        public string[32] ProcessUsername(string[32] rawInput)
        {
            variable string[32] cleanName = rawInput;
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
* Mandatory Access Modifiers: Every type, member, and definition requires an explicit access modifier.
* Mandatory Variable Types: All variable assignments start with the `variable` keyword, and all variable types are required to be explicitly declared.
* Explicitly Sized Integers: Integer and unsigned integer types must declare their exact bit size (e.g., `integer32`, `uinteger32`) rather than using a bare `integer` keyword.
* First-Class Bounded Strings: Strings carry explicit length constraints as part of their type. These constraints are checked statically whenever possible for safety, and seamlessly deferred to runtime when values are dynamic, giving the best of both static guarantees and runtime flexibility compared to standard C# strings.
  ```hazel
  import Hazel.Strings.Bounded;

  variable string[32] username = "Alice";
  ```

## 🧩 Contributing

We welcome contributions from the community! Tests and TextMate grammars are highly needed.

## 📄 License

The Hazel compiler is open-source and licensed under the [MPL-2.0 License](LICENSE).

---

<div align="center">
  <p>Copyright © 2026 Hazel Foundation</p>
  <p>🌰</p>
</div>