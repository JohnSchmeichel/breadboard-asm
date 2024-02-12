using Basm.Binding;
using Basm.Syntax;

namespace Basm;

/*
.code

loop:
    LDI 0
    ADD x
    OUT
    JMP loop

.data
    x: 27
*/

internal class Program
{
    private static void Main(string[] args)
    {
        string program = @"";
        bool verbose = false;
        bool evaluate = false;

        foreach (var arg in args)
        {
            switch (arg)
            {
                case "-v":
                    verbose = true;
                    break;
                case "eval":
                    evaluate = true;
                    break;
                default:
                    if (!File.Exists(arg))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid file or argument '{0}'", arg);
                        Console.ResetColor();
                        return;
                    }
                    program = File.ReadAllText(arg);
                    break;
            }
        }

        // Parse program
        var syntaxTree = SyntaxTree.Parse(program);

        if (verbose)
            OutputSyntaxTree(syntaxTree);

        // Check diagnostics
        if (syntaxTree.Diagnostics.Any())
            OutputDiagnostics(syntaxTree.Diagnostics);

        var boundAssembly = Binder.BindAssembly(syntaxTree);

        if (boundAssembly.Diagnostics.Any())
            OutputDiagnostics(boundAssembly.Diagnostics);

        if (verbose || evaluate)
            OutputAssemblyTree(boundAssembly);

        var breadboardIsa = new BreadboardIsa();
        var assembly = Assembly.Create(syntaxTree);

        if (evaluate)
        {
            var result = assembly.Evaluate(breadboardIsa);

            if (result.Diagnostics.Any())
                OutputDiagnostics(result.Diagnostics);

            if (result.Value != null)
                Console.WriteLine("Result: {0}", result.Value);
        }
        else
        {
            var diagnostics = assembly.Assemble(breadboardIsa);

            if (diagnostics.Any())
                OutputDiagnostics(diagnostics);
        }
    }

    private static void OutputSyntaxTree(SyntaxTree syntaxTree)
    {
        syntaxTree.WriteTo(Console.Out);
        Console.WriteLine();
    }

    private static void OutputAssemblyTree(BoundAssembly assembly)
    {
        assembly.WriteTo(Console.Out);
        Console.WriteLine();
    }

    private static void OutputDiagnostics(IEnumerable<Diagnostic> diagnostics)
    {
        Console.ForegroundColor = ConsoleColor.Red;

        foreach (var diagnostic in diagnostics)
        {
            var range = diagnostic.Location.Range;
            Console.WriteLine($"error: ({range.Start.Value}, {range.End.Value}): {diagnostic.Message}");
        }

        Console.ResetColor();
    }
}