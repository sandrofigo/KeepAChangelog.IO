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
    public Task FromFile_ValidChangelog_ToStringIsSameAsOriginal()
    {
        Changelog changelog = Changelog.FromFile("Data/valid_changelog.verified.txt");
        return Verify(changelog.ToString()).UseFileName("valid_changelog");
    }
}