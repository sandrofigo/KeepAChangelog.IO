namespace KeepAChangelog.IO.Tests;

public class ParseTests
{
    private static readonly string[] Header =
    [
        "# Changelog",
        "",
        "Description",
        ""
    ];

    private static Changelog ParseWithHeader(params string[] lines)
    {
        return Changelog.Parse([..Header, ..lines]);
    }

    [Fact]
    public void Parse_ValidChangelog_ModelIsCorrect()
    {
        Changelog changelog = Changelog.FromFile("Data/valid_changelog.verified.txt");

        changelog.Title.ShouldBe("Changelog");
        changelog.Description.ShouldStartWith("All notable changes to this project will be documented in this file.");
        changelog.Description.ShouldEndWith("[Semantic Versioning](https://semver.org/spec/v2.0.0.html).");

        changelog.Releases.Select(release => release.Version).ShouldBe(["Unreleased", "1.1.2", "1.1.1", "1.1.0", "1.0.0"]);

        Release unreleased = changelog.Releases[0];
        unreleased.ReleaseDate.ShouldBeNull();
        unreleased.IsYanked.ShouldBeFalse();

        Category changed = unreleased.Categories.Single(category => category.Type == CategoryType.Changed);
        changed.Entries.Select(entry => entry.Text).ShouldBe(["Changed 1", $"Changed 2{Environment.NewLine}Changed 2.1", "Changed 3"]);

        changelog.Releases[1].ReleaseDate.ShouldBe(new ReleaseDate(2025, 12, 1));
        changelog.Releases[1].IsYanked.ShouldBeFalse();
        changelog.Releases[2].IsYanked.ShouldBeTrue();

        changelog.VersionLinks.Count.ShouldBe(5);
        changelog.VersionLinks[0].ShouldBe(new VersionLink { Version = "unreleased", Url = "https://github.com/sandrofigo/KeepAChangelog.IO/compare/v1.1.2...HEAD" });
        changelog.VersionLinks[4].ShouldBe(new VersionLink { Version = "1.0.0", Url = "https://github.com/sandrofigo/KeepAChangelog.IO/releases/tag/v1.0.0" });
    }

    [Fact]
    public void Parse_CategoryNameCaseInsensitive_IsRecognized()
    {
        Changelog changelog = ParseWithHeader(
            "## [Unreleased]",
            "",
            "### added",
            "",
            "- A");

        Category category = changelog.Releases[0].Categories.ShouldHaveSingleItem();
        category.Type.ShouldBe(CategoryType.Added);
        category.Entries.ShouldHaveSingleItem().Text.ShouldBe("A");
    }

    [Fact]
    public void Parse_NestedBulletInEntry_IsContinuation()
    {
        Changelog changelog = ParseWithHeader(
            "## [Unreleased]",
            "",
            "### Added",
            "",
            "- A",
            "  - sub");

        Entry entry = changelog.Releases[0].Categories[0].Entries.ShouldHaveSingleItem();
        entry.Text.ShouldBe($"A{Environment.NewLine}- sub");
    }

    [Fact]
    public void Parse_NoReleases_DescriptionIsEmpty()
    {
        Changelog changelog = Changelog.Parse(Header);

        changelog.Title.ShouldBe("Changelog");
        changelog.Description.ShouldBe("");
        changelog.Releases.ShouldBeEmpty();
    }

    [Fact]
    public void Parse_EmptyReleaseFollowedByRelease_BothReleasesParsed()
    {
        Changelog changelog = ParseWithHeader(
            "## [Unreleased]",
            "",
            "## [1.0.0] - 2025-01-01",
            "",
            "### Added",
            "",
            "- A");

        changelog.Releases.Count.ShouldBe(2);
        changelog.Releases[0].Categories.ShouldBeEmpty();
        changelog.Releases[1].Categories.ShouldHaveSingleItem().Type.ShouldBe(CategoryType.Added);
    }

    [Theory]
    [InlineData("Added123")]
    [InlineData("1")]
    [InlineData("Unknown")]
    public void Parse_InvalidCategoryName_ThrowsChangelogParseException(string categoryName)
    {
        var exception = Should.Throw<ChangelogParseException>(() => ParseWithHeader(
            "## [Unreleased]",
            "",
            $"### {categoryName}",
            "",
            "- x"));

        exception.LineNumber.ShouldBe(Header.Length + 3);
        exception.Message.ShouldContain(categoryName);
    }
}
