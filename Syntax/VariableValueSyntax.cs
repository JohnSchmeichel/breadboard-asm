namespace Basm.Syntax;

public class VariableValueSyntax : SyntaxNode
{
    public VariableValueSyntax(SyntaxToken colon, SyntaxToken value)
    {
        Location = SourceLocation.From(colon.Location, value.Location);

        Colon = colon;
        Value = value;
    }

    public override SyntaxKind Kind => SyntaxKind.VariableValue;
    public override SourceLocation Location { get; }

    public SyntaxToken Colon { get; }
    public SyntaxToken Value { get; }
}
