namespace Basm;

public sealed class SourceText
{
    private readonly string text;
    private readonly string? path;

    public SourceText(string text, string? path = null)
    {
        this.text = text;
        this.path = path;
    }

    public static SourceText FromFile(string path)
    {
        return new SourceText(File.ReadAllText(path), Path.GetFullPath(path));
    }

    public string? FilePath => path;
    public string? FileName => Path.GetFileName(FilePath);

    public int Length => text.Length;
    public char this[int index] => text[index];
    public char this[Index index] => text[index];
    public SourceLocation this[Range range] => new(this, range);

    internal ReadOnlySpan<char> AsSpan(Range range) => text.AsSpan(range);
}
