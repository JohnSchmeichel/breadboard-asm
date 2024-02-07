using Basm.Syntax;

namespace Basm.Binding;

internal sealed class BoundError : BoundNode
{
    public BoundError(SyntaxNode syntax)
    {
        Syntax = syntax;
    }

    public override BoundKind Kind => BoundKind.Error;
    public override SyntaxNode Syntax { get; }

    public override string ToString() => "?";
}
