namespace Basm.Syntax;

public class InstructionSyntax : StatementSyntax
{
    public InstructionSyntax(SyntaxToken mnemonic, SyntaxToken? operand, SyntaxToken newLine)
    {
        Location = SourceLocation.From(mnemonic.Location, newLine.Location);

        Mnemonic = mnemonic;
        Operand = operand;
        NewLine = newLine;
    }

    public override SyntaxKind Kind => SyntaxKind.InstructionStatement;
    public override SourceLocation Location { get; }

    public SyntaxToken Mnemonic { get; }
    public SyntaxToken? Operand { get; }
    public SyntaxToken NewLine { get; }
}
