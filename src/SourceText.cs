using System.Collections.Immutable;

namespace Basm;

public sealed class SourceText
{
    private readonly string text;
    private readonly string? path;
    private readonly ImmutableArray<int> lines;

    public SourceText(string text, string? path = null)
    {
        this.text = text;
        this.path = path;

        var builder = ImmutableArray.CreateBuilder<int>();

        builder.Add(0);

        for (int i = 0; i < text.Length; i++)
        {
            switch (text[i])
            {
                case '\n':
                    builder.Add(i + 1);
                    break;

                case '\r':
                    if (text[i + 1] == '\n')
                        i += 1;

                    builder.Add(i + 1);
                    break;
            }
        }

        this.lines = builder.ToImmutable();
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

    internal (int Line, int Offset) GetLineAndOffset(SourceLocation location)
    {
        if (location.Text != this)
            throw new ArgumentException("Source location belongs to different source text.", nameof(location));

        int line = 0;

        while (line < lines.Length && location.Start.Value >= lines[line])
            line++;

        return (Line: line, Offset: location.Start.Value - lines[line - 1] + 1);
    }
}
