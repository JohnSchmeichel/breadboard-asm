using System.Collections.Immutable;
using Basm.Syntax;

namespace Basm.Binding;

internal sealed class Binder
{
    private readonly Diagnostics diagnostics = [];
    private readonly Dictionary<string, BoundSymbol> symbols = [];

    public Diagnostics Diagnostics => diagnostics;

    public static BoundAssembly BindAssembly(SyntaxTree syntaxTree)
    {
        var binder = new Binder();
        
        var syntax = syntaxTree.Root;
        var symbols =  syntax.Sections
            .SelectMany(x => x.Statements)
            .Where(x => x.Kind != SyntaxKind.InstructionStatement);
        
        foreach (var symbol in symbols)
        {
            binder.DeclareSymbol(symbol);
        }

        var sections = ImmutableArray.CreateBuilder<BoundSection>(syntax.Sections.Length);

        foreach (var section in syntax.Sections)
        {
            sections.Add(binder.BindSection(section));
        }

        var diagnostics = binder.Diagnostics.ToImmutableArray();

        return new BoundAssembly(syntaxTree.Root, diagnostics, sections.MoveToImmutable(), binder.symbols.Values.ToImmutableArray());
    }

    private void DeclareSymbol(StatementSyntax syntax)
    {
        switch (syntax)
        {
            case LabelSyntax label:
                DeclareLabel(label);
                break;
            case VariableSyntax variable:
                DeclareVariable(variable);
                break;
        };
    }

    private void DeclareLabel(LabelSyntax syntax)
    {
        var name = syntax.Identifier.Text ?? "?";
        var label = new BoundLabel(syntax, name);

        if (!symbols.TryAdd(name, new BoundSymbol(name, label)))
        {
            diagnostics.ReportSymbolAlreadyExists(syntax.Identifier.Location, syntax.Identifier.Text);
        }
    }

    private void DeclareVariable(VariableSyntax syntax)
    {
        var name = syntax.Identifier.Text ?? "?";
        var variable = new BoundVariable(syntax, name, syntax.ValueSyntax?.Value.Value);

        if (!symbols.TryAdd(name, new BoundSymbol(name, variable)))
        {
            diagnostics.ReportSymbolAlreadyExists(syntax.Identifier.Location, syntax.Identifier.Text);
        }
    }

    private BoundSection BindSection(SectionSyntax syntax)
    {
        var statements = ImmutableArray.CreateBuilder<BoundStatement>(syntax.Statements.Length);

        foreach (var statement in syntax.Statements)
        {
            statements.Add(BindStatement(statement));
        }

        return new BoundSection(syntax, syntax.SectionName?.Identifier.Text, statements.MoveToImmutable());
    }

    private BoundStatement BindStatement(StatementSyntax syntax)
    {
        switch (syntax.Kind)
        {
            case SyntaxKind.InstructionStatement:
                return BindInstruction((InstructionSyntax)syntax);

            case SyntaxKind.LabelStatement:
                return BindLabel((LabelSyntax)syntax);

            case SyntaxKind.VariableStatement:
                return BindVariable((VariableSyntax)syntax);
            
            default:
                throw new Exception($"Unexpected syntax {syntax.Kind}");
        }
    }

    private BoundInstruction BindInstruction(InstructionSyntax syntax)
    {
        BoundNode? operand = null;

        if (syntax.Operand is not null)
        {
            operand = syntax.Operand.Kind switch
            {
                SyntaxKind.IdentifierToken => BindSymbol(syntax.Operand),
                SyntaxKind.NumberToken => BindNumber(syntax.Operand),
                _ => new BoundError(syntax)
            };
        }

        return new BoundInstruction(syntax, syntax.Mnemonic.Text ?? "?", operand);
    }

    private BoundLabel BindLabel(LabelSyntax syntax)
    {
        return (BoundLabel)symbols[syntax.Identifier.Text ?? "?"].Node;
    }

    private BoundVariable BindVariable(VariableSyntax syntax)
    {
        return (BoundVariable)symbols[syntax.Identifier.Text ?? "?"].Node;
    }

    private BoundNode BindSymbol(SyntaxToken syntax)
    {
        if (syntax.Text is not null &&
            symbols.TryGetValue(syntax.Text, out BoundSymbol? symbol))
        {
            return symbol.Node;
        }
        else
        {
            diagnostics.ReportUndefinedSymbol(syntax.Location, syntax.Text);
            return new BoundError(syntax);
        }
    }

    private static BoundNode BindNumber(SyntaxToken syntax)
    {
        return syntax.Value is not null ?
            new BoundNumber(syntax, syntax.Value) :
            new BoundError(syntax);
    }
}
