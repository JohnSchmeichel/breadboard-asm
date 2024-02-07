using Basm.Syntax;

namespace Basm.Binding;

internal sealed class BoundNumber : BoundNode
{
    public BoundNumber(SyntaxToken syntax, object value)
    {
        Syntax = syntax;
        Value = value;
    }

    public override BoundKind Kind => BoundKind.Number;
    public override SyntaxToken Syntax { get; }

    public object Value { get; }

    public override string? ToString() => Value.ToString();
}
