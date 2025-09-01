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
}