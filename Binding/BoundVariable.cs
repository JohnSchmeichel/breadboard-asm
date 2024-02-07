using Basm.Syntax;

namespace Basm.Binding;

internal sealed class BoundVariable : BoundStatement
{
    public BoundVariable(VariableSyntax syntax, string name, object? value)
    {
        Syntax = syntax;
        Name = name;
        Value = value;
    }

    public override BoundKind Kind => BoundKind.Variable;
    public override VariableSyntax Syntax { get; }

    public string Name { get; }
    public object? Value { get; }

    public override string? ToString() => Name;
}
