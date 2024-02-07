using System.Globalization;

namespace Basm.Syntax;

internal class Lexer
{
    private readonly Diagnostics diagnostics = [];
    private readonly SourceText text;
    private int position;

    public Lexer(SourceText text)
    {
        this.text = text;
    }

    public Diagnostics Diagnostics => diagnostics;

    private char Current
    {
        get
        {
            if (position >= text.Length)
                return '\0';

            return text[position];
        }
    }

    public SyntaxToken NextToken()
    {
        var start = position;
        var kind = SyntaxKind.BadToken;
        object? value = null;

        switch (Current)
        {
            case '\0':
                kind = SyntaxKind.EndOfFileToken;
                break;
            case ',':
                kind = SyntaxKind.CommaToken;
                position++;
                break;
            case ':':
                kind = SyntaxKind.ColonToken;
                position++;
                break;
            case '.':
                kind = SyntaxKind.PeriodToken;
                position++;
                break;
            case '#':
                kind = SyntaxKind.PoundToken;
                position++;
                break;
            case ';':
                kind = ReadComment();
                break;
            case ' ':
            case '\t':
                kind = ReadWhiteSpace();
                break;
            case '\n':
            case '\r':
                kind = ReadNewLine();
                break;
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
                kind = ReadNumber(ref value);
                break;
            default:
                if (char.IsLetter(Current))
                {
                    kind = ReadIdentifier(ref value);
                }
                else if (char.IsWhiteSpace(Current))
                {
                    kind = ReadWhiteSpace();
                }
                else
                {
                    position++;
                    diagnostics.ReportBadCharacter(text[start..position], Current);
                }
                break;
        }

        return new SyntaxToken(kind, text[start..position], value);
    }

    private SyntaxKind ReadComment()
    {
        while (Current != '\0' && Current != '\n' && Current != '\r')
            position++;

        return SyntaxKind.CommentToken;
    }

    private SyntaxKind ReadWhiteSpace()
    {
        // Only reads non newline white space as newlines are separate tokens
        while (char.IsWhiteSpace(Current) && Current != '\n' && Current != '\r')
            position++;

        return SyntaxKind.WhiteSpaceToken;
    }

    private SyntaxKind ReadNewLine()
    {
        // Supports \n, \r, and \r\n and will keep separate lines separate
        switch (Current)
        {
            case '\n':
                position++;
                break;

            case '\r':
                position++;

                if (Current == '\n')
                    position++;

                break;
        }

        return SyntaxKind.NewLineToken;
    }

    private SyntaxKind ReadNumber(ref object? value)
    {
        // Supports:
        //  Binary:         0b0110
        //  Decimal:        1234
        //  Hexadecimal:    0xFF

        bool leadingZero = Current == '0';

        int start = position++;
        NumberStyles style = NumberStyles.None;

        switch (Current)
        {
            case 'b' when leadingZero:
                start = ++position;
                style = NumberStyles.AllowBinarySpecifier;

                while (Current == '0' || Current == '1')
                    position++;

                break;
            case 'x' when leadingZero:
                start = ++position;
                style = NumberStyles.AllowHexSpecifier;

                while (char.IsAsciiHexDigit(Current))
                    position++;

                break;

            default:
                while (char.IsDigit(Current))
                    position++;

                break;
        }

        var location = text[start..position];

        if (int.TryParse(location.Span, style, provider: null, out int result))
        {
            value = result;
        }
        else
        {
            diagnostics.ReportInvalidNumber(location, location.Span);
        }

        return SyntaxKind.NumberToken;
    }

    private SyntaxKind ReadIdentifier(ref object? value)
    {
        int start = position++;

        while (char.IsLetterOrDigit(Current))
            position++;

        value = text[start..position].ToString();

        return SyntaxKind.IdentifierToken;
    }
}
