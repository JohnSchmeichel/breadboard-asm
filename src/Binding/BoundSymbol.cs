namespace Basm.Binding;

internal sealed class BoundSymbol
{
    public BoundSymbol(string name, BoundNode node)
    {
        Name = name;
        Node = node;
    }

    public string Name { get; }
    public BoundNode Node { get; }
}
