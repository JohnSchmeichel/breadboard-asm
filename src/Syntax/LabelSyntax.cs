namespace Basm.Syntax;

public class LabelSyntax : StatementSyntax
{
    public LabelSyntax(SyntaxToken identifier, SyntaxToken colon)
    {
        Location = SourceLocation.From(identifier.Location, colon.Location);

        Identifier = identifier;
        Colon = colon;
    }

    public override SyntaxKind Kind => SyntaxKind.LabelStatement;
    public override SourceLocation Location { get; }

    public SyntaxToken Identifier { get; }
    public SyntaxToken Colon { get; }
}
