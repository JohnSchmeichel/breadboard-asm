namespace Basm.Syntax;

public sealed class SyntaxToken : SyntaxNode
{
    public SyntaxToken(SyntaxKind kind, SourceLocation location, object? value)
    {
        Kind = kind;
        Location = location;
        Text = location.ToString();
        Value = value;
    }

    public SyntaxToken(SyntaxKind kind, SourceLocation location)
    {
        Kind = kind;
        Location = location;
        Text = null;
        Value = null;
    }

    public override SyntaxKind Kind { get; }
    public override SourceLocation Location { get; }
    public string? Text { get; }
    public object? Value { get; }
}
