using System;

namespace KeepAChangelog.IO;

public record ReleaseDate(int Year, int Month, int Day) : IComparable<ReleaseDate>
{
    public int Year { get; } = Year;

    public int Month { get; } = Month;

    public int Day { get; } = Day;

    public override string ToString()
    {
        return $"{Year:D4}-{Month:D2}-{Day:D2}";
    }

    public int CompareTo(ReleaseDate? other)
    {
        if (ReferenceEquals(this, other))
            return 0;

        if (other is null)
            return 1;

        int yearComparison = Year.CompareTo(other.Year);
        if (yearComparison != 0)
            return yearComparison;

        int monthComparison = Month.CompareTo(other.Month);
        if (monthComparison != 0)
            return monthComparison;

        return Day.CompareTo(other.Day);
    }
}