using System.Collections.Immutable;
using Basm.Syntax;

namespace Basm.Binding;

internal sealed class BoundSection : BoundNode
{
    public BoundSection(SectionSyntax syntax, string? name, ImmutableArray<BoundStatement> boundStatements)
    {
        Syntax = syntax;
        Name = name ?? "code";
        Statements = boundStatements;
    }

    public override BoundKind Kind => BoundKind.Section;
    public override SectionSyntax Syntax { get; }

    public string Name { get; }
    public ImmutableArray<BoundStatement> Statements { get; }
}
