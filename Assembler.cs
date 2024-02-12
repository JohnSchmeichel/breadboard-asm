using System.Collections.Immutable;
using Basm.Binding;

namespace Basm;

public sealed class AssemblerOptions
{
    // Opcodes (mnemonic, opcode, instruction size)
    // Memory range
};

public sealed class Assembler
{
    private readonly Diagnostics diagnostics = [];

    public Assembler()
    {
    }

    internal IEnumerable<Diagnostic> Assemble(BoundAssembly assembly)
    {
        AssembleInstructions(assembly);

        return diagnostics.ToImmutableArray();
    }

    private void AssembleInstructions(BoundAssembly assembly)
    {
        var opcodes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["NOP"] = "0000",
            ["LDA"] = "0001",
            ["STA"] = "0010",
            ["ADD"] = "0011",
            ["SUB"] = "0100",
            ["CMP"] = "0101",
            ["LDI"] = "0110",
            ["ADI"] = "0111",
            ["SBI"] = "1000",
            ["CPI"] = "1001",
            ["JMP"] = "1010",
            ["JC"]  = "1011",
            ["JZ"]  = "1100",
            // [""] = "1101",
            ["OUT"] = "1110",
            ["HLT"] = "1111",
        };

        Range range = 0..16;
        int address = 0;

        var labels = new Dictionary<BoundLabel, int>();
        var variables = new Dictionary<BoundVariable, int>();

        // Record label addresses
        var temp = new List<BoundLabel>();

        foreach (var statement in assembly.CodeStatements)
        {
            switch (statement)
            {
                case BoundLabel label:
                    temp.Add(label);
                    break;
                case BoundInstruction instruction:
                    foreach (var label in temp)
                        labels[label] = address;
                    temp.Clear();
                    address++;
                    break;
            }
        }

        foreach (var label in temp)
            labels[label] = address;

        // Record variable addresses
        foreach (var variable in assembly.DataVariables.Reverse())
        {
            variables[variable] = range.End.Value - variables.Count - 1;
        }

        int assemblyBytes = address + variables.Count;

        if (assemblyBytes > range.End.Value)
            diagnostics.ReportAssemblyToLong(assembly.Syntax.Location, assemblyBytes, range.End.Value);

        address = 0;
        Console.WriteLine(".code");

        foreach (var instruction in assembly.InstructionStatements)
        {
            if (!opcodes.TryGetValue(instruction.Mnemonic, out string? opcode))
            {
                diagnostics.ReportUnknownMnemonic(instruction.Syntax.Mnemonic.Location, instruction.Mnemonic);
                opcode = "????";
            }

            object? value = instruction.Operand switch
            {
                // TODO: error on truncation
                BoundNumber number => GetNumber(number),
                BoundVariable variable => GetVariable(variable),
                BoundLabel label => GetLabel(label),
                null => "0000",
                _ => "????"
            };

            Console.WriteLine($"    {address:b4} {opcode}{value} ; {instruction.Mnemonic} {instruction.Operand}");
            address++;
        }

        Console.WriteLine(".data");

        foreach (var variable in assembly.DataVariables)
        {
            int value = (int)(variable.Value ?? 0);
            address = variables[variable];

            Console.WriteLine($"    {address:b4} {value:b8} ; {variable.Name}: {variable.Value ?? 0}");
        }

        string GetNumber(BoundNumber number)
        {
            const byte MaxValue = 0xF;
            int value = number.Value;

            if (value > MaxValue)
                diagnostics.ReportNumberTruncated(number.Syntax.Location, value, MaxValue);

            return (value & MaxValue).ToString("b4");
        }

        string GetVariable(BoundVariable variable)
        {
            return variables[variable].ToString("b4");
        }

        string GetLabel(BoundLabel label)
        {
            return labels[label].ToString("b4");
        }
    }
}
