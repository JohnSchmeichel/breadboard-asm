namespace Basm;

public sealed class Diagnostic
{
    public Diagnostic(SourceLocation location, string message)
    {
        Location = location;
        Message = message;
    }

    public SourceLocation Location { get; }
    public string Message { get; }

    public override string ToString() => Message;
}
