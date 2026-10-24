namespace KeepAChangelog.IO.Tests;

public class ReleaseTests
{
    [Fact]
    public void CompareTo_UnreleasedVsReleased_UnreleasedIsGreater()
    {
        var unreleased = new Release();
        var released = new Release { Version = "1.0.0", ReleaseDate = new ReleaseDate(2025, 1, 1) };

        unreleased.CompareTo(released).ShouldBeGreaterThan(0);
        released.CompareTo(unreleased).ShouldBeLessThan(0);
    }

    [Fact]
    public void CompareTo_TwoUnreleased_IsZero()
    {
        new Release().CompareTo(new Release()).ShouldBe(0);
    }

    [Fact]
    public void CompareTo_DifferentDates_LaterIsGreater()
    {
        var older = new Release { Version = "2.0.0", ReleaseDate = new ReleaseDate(2025, 1, 1) };
        var newer = new Release { Version = "1.0.0", ReleaseDate = new ReleaseDate(2025, 1, 2) };

        newer.CompareTo(older).ShouldBeGreaterThan(0);
    }

    [Fact]
    public void CompareTo_SameDateSemanticVersions_ComparesNumerically()
    {
        var lower = new Release { Version = "1.9.0", ReleaseDate = new ReleaseDate(2025, 1, 1) };
        var higher = new Release { Version = "1.10.0", ReleaseDate = new ReleaseDate(2025, 1, 1) };

        higher.CompareTo(lower).ShouldBeGreaterThan(0);
    }

    [Fact]
    public void ToString_YankedRelease_AppendsYankedSymbol()
    {
        var release = new Release { Version = "1.0.0", ReleaseDate = new ReleaseDate(2025, 1, 1), IsYanked = true };

        release.ToString().ShouldBe("## [1.0.0] - 2025-01-01 [YANKED]");
    }

    [Fact]
    public void ToString_ReleaseWithoutCategories_OnlyHeader()
    {
        new Release().ToString().ShouldBe("## [Unreleased]");
    }
}
