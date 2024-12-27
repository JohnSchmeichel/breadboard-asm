using Basm.Binding;

namespace Basm;

public abstract class InstructionSet
{
    // Opcodes
    // Registers
    // Memory

    internal abstract void Initialize(IEnumerable<BoundVariable> variable);
    internal abstract InstructionResult Evaluate(BoundInstruction instruction);
}
