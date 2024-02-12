using Basm.Syntax;

namespace Basm.Binding;

internal sealed class BoundNumber : BoundNode
{
    public BoundNumber(SyntaxToken syntax, int value)
    {
        Syntax = syntax;
        Value = value;
    }

    public override BoundKind Kind => BoundKind.Number;
    public override SyntaxToken Syntax { get; }

    public int Value { get; }

    public override string? ToString() => Value.ToString();
}
