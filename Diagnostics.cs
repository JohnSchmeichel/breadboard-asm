using System.Collections;
using Basm.Syntax;

namespace Basm;

public sealed class Diagnostics : IEnumerable<Diagnostic>
{
    private readonly List<Diagnostic> diagnostics = [];

    public IEnumerator<Diagnostic> GetEnumerator() => diagnostics.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(Diagnostics diagnostics)
    {
        this.diagnostics.AddRange(diagnostics);
    }

    public void Report(Diagnostic diagnostic)
    {
        diagnostics.Add(diagnostic);
    }

    private void Report(SourceLocation location, string message)
    {
        diagnostics.Add(new Diagnostic(location, message));
    }

    internal void ReportBadCharacter(SourceLocation location, char character)
    {
        var message = $"Unexpected input: '{character}'.";
        Report(location, message);
    }

    internal void ReportInvalidNumber(SourceLocation location, ReadOnlySpan<char> value)
    {
        var message = $"The value '{value}' is not a valid number.";
        Report(location, message);
    }

    internal void ReportUnexpectedToken(SourceLocation location, SyntaxKind actualKind, SyntaxKind expectedKind)
    {
        var message = $"Unexpected token: <{actualKind}> expected <{expectedKind}>.";
        Report(location, message);
    }

    internal void ReportUnexpectedSection(SourceLocation location, ReadOnlySpan<char> section)
    {
        var message = $"Unexpected section: '{section}'.";
        Report(location, message);
    }

    internal void ReportSymbolAlreadyExists(SourceLocation location, ReadOnlySpan<char> symbol)
    {
        var message = $"The symbol '{symbol}' already exists.";
        Report(location, message);
    }

    internal void ReportUndefinedSymbol(SourceLocation location, ReadOnlySpan<char> symbol)
    {
        var message = $"The symbol '{symbol}' does not exist.";
        Report(location, message);
    }

    internal void ReportUnknownMnemonic(SourceLocation location, ReadOnlySpan<char> mnemonic)
    {
        var message = $"The mnemonic '{mnemonic}' does not exist.";
        Report(location, message);
    }

    internal void ReportAssemblyToLong(SourceLocation location, int instructionBytes, int memoryBytes)
    {
        var message = $"The assembly is too long to fit into memory; assembly is '{instructionBytes}' bytes but memory is '{memoryBytes}'.";
        Report(location, message);
    }
}
