namespace KeepAChangelog.IO.Tests;

public class ChangelogTests
{
    static ChangelogTests()
    {
        UseSourceFileRelativeDirectory("Data");
    }

    [Fact]
    public void FromFile_ValidChangelog_NoErrors()
    {
        Changelog.FromFile("Data/valid_changelog.verified.txt");
    }

    [Fact]
    public Task ToString_ValidChangelog_IsSameAsOriginal()
    {
        Changelog changelog = Changelog.FromFile("Data/valid_changelog.verified.txt");
        return Verify(changelog.ToString()).UseFileName("valid_changelog");
    }

    [Fact]
    public Task ToString_ValidChangelogWithoutVersionLinks_IsSameAsOriginal()
    {
        Changelog changelog = Changelog.FromFile("Data/valid_changelog_without_version_links.verified.txt");
        return Verify(changelog.ToString()).UseFileName("valid_changelog_without_version_links");
    }

    [Fact]
    public Task ToString_EmptyDefaultChangelog_OutputIsCorrect()
    {
        var changelog = Changelog.Create();
        return Verify(changelog.ToString()).UseFileName("empty_changelog_from_code");
    }

    [Fact]
    public Task ToString_ChangelogWithRandomCategoryOrder_OutputIsOrdered()
    {
        Changelog changelog = Changelog.FromFile("Data/valid_changelog_with_random_category_order.txt");
        return Verify(changelog.ToString()).UseFileName("valid_changelog_with_random_category_order");
    }

    [Fact]
    public Task ToString_ChangelogWithRandomVersionOrder_OutputIsOrdered()
    {
        Changelog changelog = Changelog.FromFile("Data/valid_changelog_with_random_version_order.txt");
        return Verify(changelog.ToString()).UseFileName("valid_changelog_with_random_version_order");
    }

    [Fact]
    public void FormatFile_UnorderedFile_RewritesOrdered()
    {
        string path = Path.GetTempFileName();
        try
        {
            File.Copy("Data/valid_changelog_with_random_version_order.txt", path, true);

            Changelog.FormatFile(path);

            string expected = File.ReadAllText("Data/valid_changelog_with_random_version_order.verified.txt");
            File.ReadAllText(path).ReplaceLineEndings().ShouldBe(expected.ReplaceLineEndings());
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Theory]
    [InlineData("Data/valid_changelog.verified.txt")]
    [InlineData("Data/valid_changelog_without_version_links.verified.txt")]
    [InlineData("Data/empty_changelog_from_code.verified.txt")]
    [InlineData("Data/valid_changelog_with_random_category_order.txt")]
    [InlineData("Data/valid_changelog_with_random_version_order.txt")]
    public void IsValidFile_ParsableChangelog_ReturnsTrue(string filePath)
    {
        Changelog.IsValidFile(filePath).ShouldBeTrue();
    }

    [Fact]
    public void IsValid_UnformattedLines_ReturnsTrue()
    {
        Changelog.IsValid(
        [
            "# Changelog",
            "",
            "Description",
            "",
            "## [Unreleased]",
            "",
            "",
            "### Added",
            "",
            "- A"
        ]).ShouldBeTrue();
    }

    [Fact]
    public void IsValid_UnknownCategory_ReturnsFalse()
    {
        Changelog.IsValid(
        [
            "# Changelog",
            "",
            "Description",
            "",
            "## [Unreleased]",
            "",
            "### Unknown",
            "",
            "- A"
        ]).ShouldBeFalse();
    }

    [Fact]
    public void IsValid_EntryWithoutCategory_ReturnsFalse()
    {
        Changelog.IsValid(
        [
            "# Changelog",
            "",
            "Description",
            "",
            "## [Unreleased]",
            "",
            "- A"
        ]).ShouldBeFalse();
    }

    [Theory]
    [InlineData("Data/valid_changelog.verified.txt")]
    [InlineData("Data/valid_changelog_without_version_links.verified.txt")]
    [InlineData("Data/empty_changelog_from_code.verified.txt")]
    public void IsFormattedFile_FormattedChangelog_ReturnsTrue(string filePath)
    {
        Changelog.IsFormattedFile(filePath).ShouldBeTrue();
    }

    [Theory]
    [InlineData("Data/valid_changelog_with_random_category_order.txt")]
    [InlineData("Data/valid_changelog_with_random_version_order.txt")]
    public void IsFormattedFile_UnorderedChangelog_ReturnsFalse(string filePath)
    {
        Changelog.IsFormattedFile(filePath).ShouldBeFalse();
    }

    [Fact]
    public void IsFormattedFile_AfterFormat_ReturnsTrue()
    {
        string path = Path.GetTempFileName();
        try
        {
            File.Copy("Data/valid_changelog_with_random_version_order.txt", path, true);

            Changelog.FormatFile(path);

            Changelog.IsFormattedFile(path).ShouldBeTrue();
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Theory]
    [InlineData("\n")]
    [InlineData("\r\n")]
    public void IsFormattedFile_AnyLineEnding_ReturnsTrue(string lineEnding)
    {
        string path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, Changelog.Create().ToString().ReplaceLineEndings(lineEnding));

            Changelog.IsFormattedFile(path).ShouldBeTrue();
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void IsFormatted_CreatedChangelog_ReturnsTrue()
    {
        Changelog.IsFormatted(Changelog.Create().ToString().Split(Environment.NewLine)).ShouldBeTrue();
    }

    [Fact]
    public void IsFormatted_ExtraBlankLine_ReturnsFalse()
    {
        Changelog.IsFormatted(
        [
            "# Changelog",
            "",
            "Description",
            "",
            "## [Unreleased]",
            "",
            "",
            "### Added",
            "",
            "- A"
        ]).ShouldBeFalse();
    }

    [Fact]
    public void IsFormatted_UnorderedReleases_ReturnsFalse()
    {
        Changelog.IsFormatted(
        [
            "# Changelog",
            "",
            "Description",
            "",
            "## [1.0.0] - 2025-01-01",
            "",
            "### Added",
            "",
            "- A",
            "",
            "## [1.1.0] - 2025-02-01",
            "",
            "### Added",
            "",
            "- B"
        ]).ShouldBeFalse();
    }

    [Fact]
    public void IsFormatted_InvalidChangelog_ReturnsFalse()
    {
        Changelog.IsFormatted(
        [
            "# Changelog",
            "",
            "Description",
            "",
            "## [Unreleased]",
            "",
            "### Unknown",
            "",
            "- A"
        ]).ShouldBeFalse();
    }

    [Fact]
    public void IsFormatted_FormattedLines_ReturnsTrue()
    {
        Changelog.IsFormatted(
        [
            "# Changelog",
            "",
            "Description",
            "",
            "## [Unreleased]",
            "",
            "### Added",
            "",
            "- A"
        ]).ShouldBeTrue();
    }

    [Fact]
    public void GetUnreleased_WithUnreleasedSection_ReturnsIt()
    {
        Changelog changelog = Changelog.FromFile("Data/valid_changelog_with_random_version_order.txt");

        changelog.GetUnreleased().ShouldNotBeNull().IsUnreleased.ShouldBeTrue();
    }

    [Fact]
    public void GetUnreleased_WithoutUnreleasedSection_ReturnsNull()
    {
        Changelog changelog = Changelog.Parse(
        [
            "# Changelog",
            "",
            "Description",
            "",
            "## [1.0.0] - 2025-01-01",
            "",
            "### Added",
            "",
            "- A"
        ]);

        changelog.GetUnreleased().ShouldBeNull();
    }

    [Fact]
    public void GetLatestRelease_IgnoresUnreleased_ReturnsNewestReleased()
    {
        Changelog changelog = Changelog.Parse(
        [
            "# Changelog",
            "",
            "Description",
            "",
            "## [Unreleased]",
            "",
            "### Added",
            "",
            "- B",
            "",
            "## [1.0.0] - 2025-01-01",
            "",
            "### Added",
            "",
            "- A"
        ]);

        changelog.GetLatestRelease().ShouldNotBeNull().Version.ShouldBe("1.0.0");
    }

    [Fact]
    public void GetLatestRelease_ReleasesOutOfOrder_ReturnsNewest()
    {
        Changelog changelog = Changelog.FromFile("Data/valid_changelog_with_random_version_order.txt");

        changelog.GetLatestRelease().ShouldNotBeNull().Version.ShouldBe("1.1.2");
    }

    [Fact]
    public void GetLatestRelease_OnlyUnreleased_ReturnsNull()
    {
        Changelog.Create().GetLatestRelease().ShouldBeNull();
    }

    [Fact]
    public void ToFile_CreatedChangelog_RoundTrips()
    {
        string path = Path.GetTempFileName();
        try
        {
            Changelog.Create().ToFile(path);

            Changelog.FromFile(path).ToString().ShouldBe(Changelog.Create().ToString());
        }
        finally
        {
            File.Delete(path);
        }
    }
}