using System.Collections.Immutable;

namespace Basm.Syntax;
public class SectionSyntax : SyntaxNode
{
    public SectionSyntax(SourceLocation location, SectionNameSyntax? sectionName, ImmutableArray<StatementSyntax> statements)
    {
        Location = location;
        SectionName = sectionName;
        Statements = statements;
    }

    public override SyntaxKind Kind => SyntaxKind.Section;
    public override SourceLocation Location { get; }

    public SectionNameSyntax? SectionName { get; }
    public ImmutableArray<StatementSyntax> Statements { get; }
}
