namespace Basm.Syntax;

public class VariableSyntax : StatementSyntax
{
    public VariableSyntax(SyntaxToken identifier, VariableValueSyntax? valueSyntax, SyntaxToken newLine)
    {
        Location = identifier.Location;
        Identifier = identifier;
        ValueSyntax = valueSyntax;
        NewLine = newLine;
    }

    public override SyntaxKind Kind => SyntaxKind.VariableStatement;
    public override SourceLocation Location { get; }

    public SyntaxToken Identifier { get; }
    public VariableValueSyntax? ValueSyntax { get; }
    public SyntaxToken NewLine { get; }
}
