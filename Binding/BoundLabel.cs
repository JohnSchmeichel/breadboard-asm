using Basm.Syntax;

namespace Basm.Binding;

internal sealed class BoundLabel : BoundStatement
{
    public BoundLabel(LabelSyntax syntax, string name)
    {
        Syntax = syntax;
        Name = name;
    }

    public override BoundKind Kind => BoundKind.Label;
    public override LabelSyntax Syntax { get; }

    public string Name { get; }

    public override string ToString() => Name;
}
