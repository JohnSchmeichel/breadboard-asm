using System.Collections.Immutable;

namespace Basm.Syntax;

public class AssemblySyntax : SyntaxNode
{
    public AssemblySyntax(SourceLocation location, ImmutableArray<SectionSyntax> sections)
    {
        Location = location;
        Sections = sections;
    }

    public override SyntaxKind Kind => SyntaxKind.Assembly;
    public override SourceLocation Location { get; }

    public ImmutableArray<SectionSyntax> Sections { get; }
    public SourceText Text => Location.Text;
}
