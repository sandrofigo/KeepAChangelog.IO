namespace KeepAChangelog.IO.Tests;

public class ReleaseDateTests
{
    [Fact]
    public void ToString_SingleDigitMonthAndDay_IsZeroPadded()
    {
        new ReleaseDate(2025, 1, 5).ToString().ShouldBe("2025-01-05");
    }

    [Theory]
    [InlineData(2025, 1, 1, 2024, 12, 31)]
    [InlineData(2025, 2, 1, 2025, 1, 31)]
    [InlineData(2025, 1, 2, 2025, 1, 1)]
    public void CompareTo_LaterDate_IsGreater(int year, int month, int day, int otherYear, int otherMonth, int otherDay)
    {
        var later = new ReleaseDate(year, month, day);
        var earlier = new ReleaseDate(otherYear, otherMonth, otherDay);

        later.CompareTo(earlier).ShouldBeGreaterThan(0);
        earlier.CompareTo(later).ShouldBeLessThan(0);
    }

    [Fact]
    public void CompareTo_EqualDates_IsZero()
    {
        new ReleaseDate(2025, 1, 1).CompareTo(new ReleaseDate(2025, 1, 1)).ShouldBe(0);
    }

    [Fact]
    public void CompareTo_Null_IsGreater()
    {
        new ReleaseDate(2025, 1, 1).CompareTo(null).ShouldBe(1);
    }

    [Fact]
    public void ToDateTime_FromDateTime_RoundTrip()
    {
        ReleaseDate releaseDate = ReleaseDate.FromDateTime(new DateTime(2025, 3, 4, 13, 14, 15));

        releaseDate.ShouldBe(new ReleaseDate(2025, 3, 4));
        releaseDate.ToDateTime().ShouldBe(new DateTime(2025, 3, 4));
    }

    [Fact]
    public void ToDateOnly_FromDateOnly_RoundTrip()
    {
        ReleaseDate releaseDate = ReleaseDate.FromDateOnly(new DateOnly(2025, 3, 4));

        releaseDate.ShouldBe(new ReleaseDate(2025, 3, 4));
        releaseDate.ToDateOnly().ShouldBe(new DateOnly(2025, 3, 4));
    }
}
