using System.Collections.Immutable;
using Basm.Syntax;

namespace Basm.Binding;

internal sealed class BoundAssembly
{
    public BoundAssembly(AssemblySyntax syntax, ImmutableArray<Diagnostic> diagnostics, ImmutableArray<BoundSection> sections, ImmutableArray<BoundSymbol> symbols)
    {
        Syntax = syntax;
        Diagnostics = diagnostics;
        Sections = sections;
        Symbols = symbols;
    }

    public SourceText Text => Syntax.Text;
    public AssemblySyntax Syntax { get; }
    public IEnumerable<Diagnostic> Diagnostics { get; }
    public ImmutableArray<BoundSection> Sections { get; }
    public ImmutableArray<BoundSymbol> Symbols { get; }

    public IEnumerable<BoundSection> CodeSections => Sections.Where(x => x.Name == "code");
    public IEnumerable<BoundSection> DataSections => Sections.Where(x => x.Name == "data");

    public IEnumerable<BoundStatement> CodeStatements => CodeSections.SelectMany(x => x.Statements);
    public IEnumerable<BoundInstruction> InstructionStatements => CodeSections.SelectMany(x => x.Statements).OfType<BoundInstruction>();
    public IEnumerable<BoundVariable> DataVariables => DataSections.SelectMany(x => x.Statements).OfType<BoundVariable>();
}
