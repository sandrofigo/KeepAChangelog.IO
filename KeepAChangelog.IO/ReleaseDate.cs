namespace KeepAChangelog.IO;

public record ReleaseDate(int Year, int Month, int Day)
{
    public int Year { get; } = Year;

    public int Month { get; } = Month;

    public int Day { get; } = Day;

    public override string ToString()
    {
        return $"{Year:D4}-{Month:D2}-{Day:D2}";
    }
}