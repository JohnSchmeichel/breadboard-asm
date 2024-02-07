using System.Collections.Immutable;

namespace Basm.Syntax;

public sealed class SyntaxTree
{
    private SyntaxTree(SourceText text, AssemblySyntax root, ImmutableArray<Diagnostic> diagnostics)
    {
        Text = text;
        Root = root;
        Diagnostics = diagnostics;
    }

    public SourceText Text { get; }
    public AssemblySyntax Root { get; }
    public IEnumerable<Diagnostic> Diagnostics { get; }

    public static SyntaxTree Parse(string text)
    {
        var sourceText = new SourceText(text);
        var parser = new Parser(sourceText);
        var root = parser.Parse();
        var diagnostics = parser.Diagnostics.ToImmutableArray();

        return new SyntaxTree(sourceText, root, diagnostics);
    }
}
