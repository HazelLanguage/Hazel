using System;
using System.IO;
using Hazel.Compiler;
using Hazel.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  hazel <path-to-file>");
            Console.WriteLine("  hazel -c \"var h = 8;\"");
            return;
        }

        string source = string.Empty;

        if (args[0] == "-c" && args.Length > 1)
        {
            source = args[1];
        }
        else
        {
            string absolutePath = Path.GetFullPath(args[0]);
            if (!File.Exists(absolutePath))
            {
                string? compilerLocation = Environment.ProcessPath;
                Console.WriteLine($"{compilerLocation}: can't open file '{absolutePath}': [{ErrorCodes.FileNotFound}] No such file or directory");
                return;
            }
            source = File.ReadAllText(absolutePath);
        }

        var compiler = new Compiler();
        string output = compiler.Compile(source);
        Console.WriteLine(output);
    }
}