namespace Basm.Syntax;

public class SectionNameSyntax : SyntaxNode
{
    public SectionNameSyntax(SyntaxToken period, SyntaxToken identifier)
    {
        Location = SourceLocation.From(period.Location, identifier.Location);

        Period = period;
        Identifier = identifier;
    }

    public override SyntaxKind Kind => SyntaxKind.SectionName;
    public override SourceLocation Location { get; }

    public SyntaxToken Period { get; }
    public SyntaxToken Identifier { get; }
}
