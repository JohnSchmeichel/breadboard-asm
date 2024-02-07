namespace Basm;

public sealed class SourceText
{
    private readonly string text;

    public SourceText(string text)
    {
        this.text = text;
    }

    public string? FileName { get; }

    public int Length => text.Length;
    public char this[int index] => text[index];
    public char this[Index index] => text[index];
    public SourceLocation this[Range range] => new(this, range);

    internal ReadOnlySpan<char> AsSpan(Range range) => text.AsSpan(range);
}
