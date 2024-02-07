namespace Basm.Syntax;

public abstract class SyntaxNode
{
    public abstract SyntaxKind Kind { get; }
    public abstract SourceLocation Location { get; }
}
