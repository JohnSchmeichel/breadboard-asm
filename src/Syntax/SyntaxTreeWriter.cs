namespace Basm.Syntax;

public static class SyntaxTreeWriter
{
    private const string None = "    ";
    private const string More = "│   ";
    private const string Next = "├───";
    private const string Last = "└───";

    public static void WriteTo(this SyntaxTree syntax, TextWriter writer)
    {
        try
        {
            WriteAssemblySyntax(writer, syntax.Root, Last);
        }
        finally
        {
            ResetColor(writer);
        }
    }

    private static void WriteAssemblySyntax(TextWriter writer, AssemblySyntax syntax, string indent)
    {
        WriteNode(writer, syntax, indent);
        indent = GetIndent(indent);

        WriteNodes(writer, syntax.Sections, indent);
    }

    private static void WriteSectionSyntax(TextWriter writer, SectionSyntax syntax, string indent)
    {
        WriteNode(writer, syntax, indent);
        indent = GetIndent(indent);

        if (syntax.SectionName != null)
            WriteSectionNameSyntax(writer, syntax.SectionName, indent + (syntax.Statements.Any() ? Next : Last));

        WriteNodes(writer, syntax.Statements, indent);
    }

    private static void WriteSectionNameSyntax(TextWriter writer, SectionNameSyntax syntax, string indent)
    {
        WriteNode(writer, syntax, indent);
        indent = GetIndent(indent);

        WriteNode(writer, syntax.Period, indent + Next);
        WriteNode(writer, syntax.Identifier, indent + Last);
    }

    private static void WriteLabelSyntax(TextWriter writer, LabelSyntax syntax, string indent)
    {
        WriteNode(writer, syntax, indent);
        indent = GetIndent(indent);

        WriteNode(writer, syntax.Identifier, indent + Next);
        WriteNode(writer, syntax.Colon, indent + Last);
    }

    private static void WriteInstructionSyntax(TextWriter writer, InstructionSyntax syntax, string indent)
    {
        WriteNode(writer, syntax, indent);
        indent = GetIndent(indent);

        WriteNode(writer, syntax.Mnemonic, indent + Next);

        if (syntax.Operand != null)
            WriteNode(writer, syntax.Operand, indent + Next);
        
        WriteNode(writer, syntax.NewLine, indent + Last);
    }

    private static void WriteVariableSyntax(TextWriter writer, VariableSyntax syntax, string indent)
    {
        WriteNode(writer, syntax, indent);
        indent = GetIndent(indent);

        WriteNode(writer, syntax.Identifier, indent + Next);

        if (syntax.ValueSyntax != null)
            WriteVariableValueSyntax(writer, syntax.ValueSyntax, indent + Next);
        
        WriteNode(writer, syntax.NewLine, indent + Last);
    }

    private static void WriteVariableValueSyntax(TextWriter writer, VariableValueSyntax syntax, string indent)
    {
        WriteNode(writer, syntax, indent);
        indent = GetIndent(indent);

        WriteNode(writer, syntax.Colon, indent + Next);
        WriteNode(writer, syntax.Value, indent + Last);
    }

    private static void WriteNode(TextWriter writer, SyntaxNode node, string indent)
    {
        SyntaxToken? token = node as SyntaxToken;

        SetForeground(writer, ConsoleColor.DarkGray);
        writer.Write(indent);

        SetForeground(writer, token == null ? ConsoleColor.Cyan : ConsoleColor.Blue);
        writer.Write(node.Kind);

        if (token?.Value != null)
        {
            writer.Write(" ");

            SetForeground(writer, token.Kind == SyntaxKind.NumberToken ? ConsoleColor.Magenta : ConsoleColor.Gray);
            writer.Write(token.Value);
        }

        writer.WriteLine();
    }

    private static void WriteNodes<T>(TextWriter writer, IReadOnlyList<T> nodes, string indent)
        where T: SyntaxNode
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            var node = nodes[i];
            var last = i == nodes.Count - 1;
            var nextIndent = indent + (last ? Last : Next);

            switch (node)
            {
                case AssemblySyntax syntax:
                    WriteAssemblySyntax(writer, syntax, nextIndent);
                    break;
                case SectionSyntax syntax:
                    WriteSectionSyntax(writer, syntax, nextIndent);
                    break;
                case LabelSyntax syntax:
                    WriteLabelSyntax(writer, syntax, nextIndent);
                    break;
                case InstructionSyntax syntax:
                    WriteInstructionSyntax(writer, syntax, nextIndent);
                    break;
                case VariableSyntax syntax:
                    WriteVariableSyntax(writer, syntax, nextIndent);
                    break;
                default:
                    throw new NotImplementedException($"Unknown node type {node.GetType()}");
            };
        }
    }

    private static string GetIndent(string indent)
    {
        return indent[0..^4] + (indent.EndsWith(Last) ? None : More);
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
