﻿using Basm.Binding;
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

        if (args.Length > 0)
            program = File.ReadAllText(args[0]);

        // Parse program
        var syntaxTree = SyntaxTree.Parse(program);

        OutputSyntaxTree(syntaxTree);

        // Check diagnostics
        if (syntaxTree.Diagnostics.Any())
            OutputDiagnostics(syntaxTree.Diagnostics);

        var boundAssembly = Binder.BindAssembly(syntaxTree);

        if (boundAssembly.Diagnostics.Any())
            OutputDiagnostics(boundAssembly.Diagnostics);

        OutputAssemblyTree(boundAssembly);

        var breadboardIsa = new BreadboardIsa();
        var assembly = Assembly.Create(syntaxTree);

        var diagnostics = assembly.Assemble(breadboardIsa);

        if (diagnostics.Any())
            OutputDiagnostics(diagnostics);

        var result = assembly.Evaluate(breadboardIsa);

        if (result.Diagnostics.Any())
            OutputDiagnostics(result.Diagnostics);
        
        if (result.Value != null)
            Console.WriteLine("Result: {0}", result.Value);
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