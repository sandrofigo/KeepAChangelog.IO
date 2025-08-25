namespace KeepAChangelog.IO.Tests;

public class ChangelogTests
{
    [Fact]
    public void FromFile_ValidChangelog_NoErrors()
    {
        Changelog.FromFile("Data/valid_changelog.md");
    }
}
