using Basm.Binding;

namespace Basm;

public class BreadboardIsa : InstructionSet
{
    private readonly Dictionary<BoundVariable, byte> memory = [];
    private byte registerA;
    private byte registerB;
    private byte output;
    private bool flagCarry;
    private bool flagZero;

    internal override void Initialize(IEnumerable<BoundVariable> variables)
    {
        memory.Clear();
        registerA = 0;
        registerB = 0;
        output = 0;
        flagCarry = false;
        flagZero = false;

        foreach (var variable in variables)
        {
            memory[variable] = (byte)((int?)variable.Value ?? 0);
        }
    }

    internal override InstructionResult Evaluate(BoundInstruction instruction)
    {
        BoundVariable var;
        BoundLabel? label;

        switch (instruction.Mnemonic)
        {
            case "NOP":
                return default;
            case "LDA":
                var = GetVariable(instruction);     // IO|MI
                registerA = memory[var];            // RO|AI
                return default;
            case "STA":
                var = GetVariable(instruction);     // IO|MI
                memory[var] = registerA;            // AO|RI
                return default;
            case "ADD":
                var = GetVariable(instruction);     // IO|MI
                registerB = memory[var];            // RO|BI
                Add();                              // SO|AI|FI
                return default;
            case "SUB":
                var = GetVariable(instruction);     // IO|MI
                registerB = memory[var];            // RO|BI
                Subtract();                         // SO|SU|AI|FI
                return default;
            case "LDI":
                registerA = GetByte(instruction);   // IO|AI
                return default;
            case "ADI":
                registerB = GetByte(instruction);   // IO|BI
                Add();                              // SO|AI|FI
                return default;
            case "SBI":
                registerB = GetByte(instruction);   // IO|BI
                Subtract();                         // SO|SU|AI|FI
                return default;
            case "CMP":
                var = GetVariable(instruction);     // IO|MI
                registerB = memory[var];            // RO|BI
                Compare();                          // SU|FI
                return default;
            case "CPI":
                registerB = GetByte(instruction);   // IO|BI
                Compare();                          // SU|FI
                return default;
            case "JMP":
                label = GetLabel(instruction);      // IO|CI
                return InstructionResult.Jump(label);
            case "JC":
                label = flagCarry ? GetLabel(instruction) : null;
                return InstructionResult.Jump(label);
            case "JZ":
                label = flagZero ? GetLabel(instruction) : null;
                return InstructionResult.Jump(label);
            case "OTI":
                Console.WriteLine("Output: {0}", output = GetByte(instruction));
                return default;
            case "OUT":
                Console.WriteLine("Output: {0}", output = registerA);
                return default;
            case "HLT":
                Console.WriteLine("Memory:");
                foreach (var variable in memory)
                    Console.WriteLine("  {0}: {1}", variable.Key.Name, variable.Value);
                return InstructionResult.Halt(output);
            default:
                Console.WriteLine($"Unknown mnemonic {instruction.Mnemonic}");
                return InstructionResult.Invalid(instruction);
        }

        void Add()
        {
            int sum = registerA + registerB;
            registerA = (byte)sum;
            flagCarry = sum > byte.MaxValue;
            flagZero = registerA == 0;
        }

        void Subtract()
        {
            int sub = registerA + ~registerB + 1;
            registerA = (byte)sub;
            flagCarry = sub < 0;
            flagZero = registerA == 0;
        }

        void Compare()
        {
            int sub = registerA + ~registerB + 1;
            int value = (byte)sub;
            flagCarry = sub < 0;
            flagZero = value == 0;
        }

        static byte GetByte(BoundInstruction instruction)
        {
            return (byte)((BoundNumber)instruction.Operand!).Value;
        }

        static BoundLabel GetLabel(BoundInstruction instruction)
        {
            return (BoundLabel)instruction.Operand!;
        }

        static BoundVariable GetVariable(BoundInstruction instruction)
        {
            return (BoundVariable)instruction.Operand!;
        }
    }
}
