using System.Collections.Immutable;

namespace Basm.Syntax;

internal class Parser
{
    private readonly Diagnostics diagnostics = [];
    private readonly ImmutableArray<SyntaxToken> tokens;
    private readonly SourceText text;
    private int position;

    public Parser(SourceText text)
    {
        var tokens = ImmutableArray.CreateBuilder<SyntaxToken>();
        var lexer = new Lexer(text);

        SyntaxToken token;
        do
        {
            token = lexer.NextToken();

            if (token.Kind != SyntaxKind.WhiteSpaceToken &&
                token.Kind != SyntaxKind.CommentToken &&
                token.Kind != SyntaxKind.BadToken)
            {
                tokens.Add(token);
            }
        }
        while (token.Kind != SyntaxKind.EndOfFileToken);

        this.tokens = tokens.ToImmutable();
        this.diagnostics.Add(lexer.Diagnostics);
        this.text = text;
    }

    public Diagnostics Diagnostics => diagnostics;
    public IEnumerable<SyntaxToken> Tokens => tokens;

    private SyntaxToken Current => Peek();

    public AssemblySyntax Parse()
    {
        // Assembly
        //  zero or more sections (implicit code section)
        //  zero or more instructions
        //  each instruction has an optional set of operands

        return ParseAssembly();
    }

    private SyntaxToken Peek()
    {
        if (position >= tokens.Length)
            return tokens[^1];

        return tokens[position];
    }


    private SyntaxToken NextToken()
    {
        var current = Current;
        position++;
        return current;
    }

    private SyntaxToken MatchToken(SyntaxKind kind)
    {
        if (Current.Kind == kind)
            return NextToken();
        
        // New line tokens are used to indicate the end of statements, but one may not exist
        // before the end of the file. Special case this here to return the end of file token
        if (Current.Kind == SyntaxKind.EndOfFileToken && kind == SyntaxKind.NewLineToken)
            return Current;

        diagnostics.ReportUnexpectedToken(Current.Location, Current.Kind, kind);

        return new SyntaxToken(kind, Current.Location);
    }

    private AssemblySyntax ParseAssembly()
    {
        var start = Index.Start;
        var sections = ImmutableArray.CreateBuilder<SectionSyntax>();

        while (Current.Kind != SyntaxKind.EndOfFileToken)
        {
            while (Current.Kind == SyntaxKind.NewLineToken)
                NextToken();
            
            var startToken = Current;

            var section = ParseSection();
            sections.Add(section);

            // If ParseSection() did not consume any tokens,
            // we need to skip the current token and continue
            // in order to avoid an infinite loop.
            if (Current == startToken)
                NextToken();
        }

        var end = Current.Location.Start;

        return new AssemblySyntax(text[start..end], sections.ToImmutable());
    }

    private SectionSyntax ParseSection()
    {
        // Parse optional section name
        // Parse section statements

        var start = Current.Location.Start;

        SectionNameSyntax? sectionName = ParseOptionalSectionName();
        ImmutableArray<StatementSyntax> lines;

        var section = sectionName?.Identifier.Text ?? "code";

        switch (section)
        {
            case "code":
                lines = ParseStatements(ParseStatement);
                break;
            case "data":
                lines = ParseStatements(ParseVariable);
                break;
            default:
                diagnostics.ReportUnexpectedSection(sectionName!.Identifier.Location, section);
                lines = [];
                break;
        }

        var end = Current.Location.Start;

        return new SectionSyntax(text[start..end], sectionName, lines);

        SectionNameSyntax? ParseOptionalSectionName()
        {
            if (Current.Kind != SyntaxKind.PeriodToken)
                return null;
            
            SyntaxToken period = MatchToken(SyntaxKind.PeriodToken);
            SyntaxToken identifier = MatchToken(SyntaxKind.IdentifierToken);

            return new SectionNameSyntax(period, identifier);
        }
    }

    private ImmutableArray<StatementSyntax> ParseStatements(Func<StatementSyntax> ParseStatement)
    {
        // Parse statements
        var statements = ImmutableArray.CreateBuilder<StatementSyntax>();

        while (Current.Kind != SyntaxKind.EndOfFileToken)
        {
            while (Current.Kind == SyntaxKind.NewLineToken)
                NextToken();

            // Keep parsing lines until section directive or end of file
            if (Current.Kind == SyntaxKind.PeriodToken ||
                Current.Kind == SyntaxKind.EndOfFileToken)
                break;
            
            var startToken = Current;

            var statement = ParseStatement();
            statements.Add(statement);

            // If ParseStatement() did not consume any tokens,
            // we need to skip the current token and continue
            // in order to avoid an infinite loop.
            if (Current == startToken)
                NextToken();
        }

        return statements.ToImmutable();
    }

    private StatementSyntax ParseStatement()
    {
        var identifier = MatchToken(SyntaxKind.IdentifierToken);

        switch (Current.Kind)
        {
            case SyntaxKind.ColonToken:
                return ParseLabel(identifier);
            default:
                return ParseInstruction(identifier);
        }
    }

    private LabelSyntax ParseLabel(SyntaxToken identifier)
    {
        // Parse identifier
        // Parse colon

        var colon = MatchToken(SyntaxKind.ColonToken);

        return new LabelSyntax(identifier, colon);
    }

    private InstructionSyntax ParseInstruction(SyntaxToken identifier)
    {
        // Parse mnemonic
        // Parse operand
        // Parse new line

        SyntaxToken? operand = null;

        if (Current.Kind != SyntaxKind.NewLineToken &&
            Current.Kind != SyntaxKind.EndOfFileToken)
        {
            operand = NextToken();
        }

        var newLine = MatchToken(SyntaxKind.NewLineToken);

        return new InstructionSyntax(identifier, operand, newLine);
    }

    private VariableSyntax ParseVariable()
    {
        // Parse identifier
        // Parse option value
        // Parse new line

        var identifier = MatchToken(SyntaxKind.IdentifierToken);
        var value = ParseOptionalValue();
        var newLine = MatchToken(SyntaxKind.NewLineToken);

        return new VariableSyntax(identifier, value, newLine);

        VariableValueSyntax? ParseOptionalValue()
        {
            if (Current.Kind != SyntaxKind.ColonToken &&
                Current.Kind != SyntaxKind.NumberToken)
                return null;
            
            var colon = MatchToken(SyntaxKind.ColonToken);
            var number = MatchToken(SyntaxKind.NumberToken);

            return new VariableValueSyntax(colon, number);
        }
    }
}
