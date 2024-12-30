namespace Basm.Syntax;

public enum SyntaxKind
{
    // Tokens
    BadToken,
    EndOfFileToken,
    WhiteSpaceToken,
    NewLineToken,
    NumberToken,
    CommaToken,
    ColonToken,
    PeriodToken,
    PoundToken,
    IdentifierToken,
    CommentToken,

    // Keywords
    CodeKeyword,
    DataKeyword,

    // Nodes
    Assembly,
    Section,
    SectionName,
    VariableValue,

    // Statements
    LabelStatement,
    InstructionStatement,
    VariableStatement,
}
