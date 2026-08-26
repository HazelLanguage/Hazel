# Hazel

![Hazel Logo](https://raw.githubusercontent.com/HazelLanguage/Hazel/main/assets/Hazel_Logo.webp)

A modern, high performance programming language designed for building scalable, efficient, and enterprise-grade applications.

## 📦 Installation

Install the CLI tool from NuGet:

```powershell
dotnet tool install --global Hazel
```

or build and install from source:

```powershell
dotnet pack
dotnet tool install --global --source ./src/Hazel/bin/Release Hazel
```

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
    internal class Calculator
    {
        private protected int Add(int a, int b)
        {
            var h = 8;
            return a + b * h;
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

* Mandatory Access Modifiers: Every type, member, and definition requires an explicit access modifier.
* First-Class Bounded Strings: Strings with explicit length constraints are first-class citizens.

## 📄 License

The Hazel compiler is open-source and licensed under the [MPL-2.0 License](LICENSE).

---

<div align="center">
  <p>Copyright © 2026 Hazel Foundation</p>
  <p>🌰</p>
</div>