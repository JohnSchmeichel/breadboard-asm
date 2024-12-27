using Basm.Binding;

namespace Basm;

public enum OperandType
{
}

public readonly struct Operand
{
    public byte Size { get; init; }
    
    // Type: register, memory, immediate
}

public readonly struct OpCode
{
    public string Mnemonic { get; init; }
    public byte Value { get; init; }
    public byte Size { get; init; }
    public IReadOnlyList<Operand> Operands { get; init; }
}

public abstract class InstructionSet
{
    // Opcodes
    // Registers
    // Memory

    private static OpCode[] OpCodes = new[]
    {
        new OpCode { Mnemonic = "NOP", Value = 0b0000, Size = 8 },
        new OpCode { Mnemonic = "LDA", Value = 0b0001, Size = 8 },
        new OpCode { Mnemonic = "OUT", Value = 0b1110, Size = 8 },
        new OpCode { Mnemonic = "OUT", Value = 0b1110, Size = 8, Operands = [new Operand { Size = 4 }] },
        new OpCode { Mnemonic = "HLT", Value = 0b1111, Size = 8 },
    };

    internal abstract void Initialize(IEnumerable<BoundVariable> variable);
    internal abstract InstructionResult Evaluate(BoundInstruction instruction);
}
