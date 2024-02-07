namespace Basm;

public readonly struct SourceLocation
{
    public SourceLocation(SourceText text, Range range)
    {
        Text = text;
        Range = range;
    }

    public static SourceLocation From(SourceLocation start, SourceLocation end)
    {
        if (start.Text != end.Text)
            throw new ArgumentException("Start and end locations are from different sources.");

        return new(start.Text, start.Range.Start..end.Range.End);
    }

    public SourceText Text { get; }
    public Range Range { get; }
    public ReadOnlySpan<char> Span => Text.AsSpan(Range);

    public Index Start => Range.Start;
    public Index End => Range.End;

    public override string ToString() => Span.ToString();
}
