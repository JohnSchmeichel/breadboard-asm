using Basm.Syntax;

namespace Basm.Binding;

internal sealed class BoundInstruction : BoundStatement
{
    public BoundInstruction(InstructionSyntax syntax, string mnemonic, BoundNode? operand)
    {
        Syntax = syntax;
        Mnemonic = mnemonic;
        Operand = operand;
    }

    public override BoundKind Kind => BoundKind.Instruction;
    public override InstructionSyntax Syntax { get; }

    public string Mnemonic { get; }
    public BoundNode? Operand { get; }
}
