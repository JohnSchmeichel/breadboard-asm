using Basm.Syntax;

namespace Basm.Binding;

internal abstract class BoundNode
{
    public abstract BoundKind Kind { get; }
    public abstract SyntaxNode Syntax { get; }
}
