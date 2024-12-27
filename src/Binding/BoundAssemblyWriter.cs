namespace Basm.Binding;

internal static class BoundAssemblyWriter
{
    public static void WriteTo(this BoundAssembly assembly, TextWriter writer)
    {
        try
        {
            WriteAssembly(writer, assembly);
        }
        finally
        {
            ResetColor(writer);
        }
    }

    private static void WriteAssembly(TextWriter writer, BoundAssembly assembly)
    {
        var source = assembly.Text.FileName ?? "[Assembly]";
        
        SetForeground(writer, ConsoleColor.DarkGray);
        writer.Write(source);
        writer.Write(':');

        ResetColor(writer);
        writer.WriteLine();

        foreach (var section in assembly.Sections)
        {
            WriteSection(writer, section);
        }
    }

    private static void WriteSection(TextWriter writer, BoundSection section)
    {
        SetForeground(writer, ConsoleColor.DarkGray);
        writer.Write('.');

        SetForeground(writer, ConsoleColor.Blue);
        writer.Write(section.Name);

        ResetColor(writer);
        writer.WriteLine();

        foreach (var statement in section.Statements)
        {
            switch (statement)
            {
                case BoundLabel label:
                    WriteLabel(writer, label);
                    break;
                case BoundInstruction instruction:
                    WriteInstruction(writer, instruction);
                    break;
                case BoundVariable variable:
                    WriteVariable(writer, variable);
                    break;
                default:
                    SetForeground(writer, ConsoleColor.Red);
                    writer.WriteLine("    <unknown statement>: {0}", statement.Kind);
                    ResetColor(writer);
                    break;
            }
        }
    }

    private static void WriteLabel(TextWriter writer, BoundLabel label)
    {
        SetForeground(writer, ConsoleColor.Cyan);
        writer.Write(label.Name);

        SetForeground(writer, ConsoleColor.DarkGray);
        writer.Write(':');

        ResetColor(writer);
        writer.WriteLine();
    }

    private static void WriteInstruction(TextWriter writer, BoundInstruction instruction)
    {
        writer.Write("    ");
        SetForeground(writer, ConsoleColor.Gray);
        writer.Write(instruction.Mnemonic);

        if (instruction.Operand != null)
        {
            writer.Write(' ');
            
            switch (instruction.Operand)
            {
                case BoundLabel label:
                    SetForeground(writer, ConsoleColor.Cyan);
                    writer.Write(label.Name);
                    break;
                case BoundVariable variable:
                    SetForeground(writer, ConsoleColor.DarkGray);
                    writer.Write(variable.Name);
                    break;
                case BoundNumber number:
                    SetForeground(writer, ConsoleColor.Magenta);
                    writer.Write(number.Value);
                    break;
                default:
                    SetForeground(writer, ConsoleColor.Red);
                    writer.Write("<unknown operand>: {0}", instruction.Operand.Kind);
                    break;
            }
        }

        ResetColor(writer);
        writer.WriteLine();
    }

    private static void WriteVariable(TextWriter writer, BoundVariable variable)
    {
        writer.Write("    ");
        SetForeground(writer, ConsoleColor.Gray);
        writer.Write(variable.Name);

        if (variable.Value != null)
        {
            SetForeground(writer, ConsoleColor.DarkGray);
            writer.Write(':');

            ResetColor(writer);
            writer.Write(' ');

            SetForeground(writer, ConsoleColor.Magenta);
            writer.Write(variable.Value);
        }

        ResetColor(writer);
        writer.WriteLine();
    }

    private static void SetForeground(TextWriter writer, ConsoleColor color)
    {
        if (writer == Console.Out)
            Console.ForegroundColor = color;
    }

    private static void ResetColor(TextWriter writer)
    {
        if (writer == Console.Out)
            Console.ResetColor();
    }
}