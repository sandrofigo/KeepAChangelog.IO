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
    public void Format_UnorderedFile_RewritesOrdered()
    {
        string path = Path.GetTempFileName();
        try
        {
            File.Copy("Data/valid_changelog_with_random_version_order.txt", path, true);

            Changelog.Format(path);

            string expected = File.ReadAllText("Data/valid_changelog_with_random_version_order.verified.txt");
            File.ReadAllText(path).ReplaceLineEndings().ShouldBe(expected.ReplaceLineEndings());
        }
        finally
        {
            File.Delete(path);
        }
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