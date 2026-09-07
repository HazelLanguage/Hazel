using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Hazel.Compiler;
using Hazel.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  hazel <path-to-file>");
            Console.WriteLine("  hazel -c \"variable integer32 h = 8;\"");
            return;
        }

        bool emitCSharpOnly = false;
        string source = string.Empty;

#pragma warning disable CRRSP05, CRRSP06
        var argList = args.ToList();
        if (argList.Contains("--transpile") || argList.Contains("-t"))
        {
            emitCSharpOnly = true;
            argList.Remove("--transpile");
            argList.Remove("-t");
        }
#pragma warning restore CRRSP05, CRRSP06

        if (argList.Count > 0 && argList[0] == "-c")
        {
            if (argList.Count > 1)
            {
                source = string.Join(
                    Environment.NewLine,
                    argList.Skip(1));
            }
            else
            {
                Console.WriteLine("Error: -c option requires an inline code string.");
                return;
            }
        }
        else if (argList.Count > 0)
        {
            string absolutePath = Path.GetFullPath(argList[0]);
            if (!File.Exists(absolutePath))
            {
                string? compilerLocation = Environment.ProcessPath;
                Console.WriteLine($"{compilerLocation}: can't open file '{absolutePath}': [{ErrorCodes.FileNotFound}] No such file or directory");
                return;
            }
            source = File.ReadAllText(absolutePath);
        }
        else
        {
            Console.WriteLine("Error: No source file or inline code provided.");
            return;
        }

        var compiler = new Compiler();
        string transpiledCSharp = compiler.Compile(source);

        if (emitCSharpOnly)
        {
            Console.WriteLine(transpiledCSharp);
        }
        else
        {
            ExecuteCSharp(transpiledCSharp);
        }
    }

    static void ExecuteCSharp(string csharpCode)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(csharpCode);

        string assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
        var references = new MetadataReference[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
            MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll"))
        };

        var compilation = CSharpCompilation.Create(
            assemblyName: Path.GetRandomFileName(),
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: new CSharpCompilationOptions(
                OutputKind.ConsoleApplication,
                allowUnsafe: true
            )
        );

        using var peStream = new MemoryStream();
        var emitResult = compilation.Emit(peStream);

        if (!emitResult.Success)
        {
            foreach (var diagnostic in emitResult.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error))
            {
                Console.WriteLine(diagnostic.ToString());
            }
            return;
        }

        peStream.Seek(0, SeekOrigin.Begin);

        var assembly = AssemblyLoadContext.Default.LoadFromStream(peStream);
        var entryPoint = assembly.EntryPoint;

        if (entryPoint == null)
        {
            Console.WriteLine("Error: No entry point (Main method) found in compiled code.");
            return;
        }

        object?[]? parameters = entryPoint.GetParameters().Length > 0 ? new object[] { Array.Empty<string>() } : null;
        entryPoint.Invoke(null, parameters);
    }
}