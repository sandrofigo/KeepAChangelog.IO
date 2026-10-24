namespace KeepAChangelog.IO.Tests;

public class VersionLinkTests
{
    [Theory]
    // Semantic versions
    [InlineData("1.10.0", "1.9.0")]
    [InlineData("2.0.0", "1.99.99")]
    [InlineData("1.0.0", "1.0.0-beta")]
    [InlineData("1.0.0-beta.11", "1.0.0-beta.2")]
    [InlineData("1.0.0-beta", "1.0.0-alpha.1")]
    [InlineData("1.0.0-alpha.1", "1.0.0-alpha")]
    [InlineData("v1.10.0", "v1.9.0")]
    // Non-semantic versions
    [InlineData("unreleased", "1.0.0")]
    [InlineData("1.10", "1.9")]
    [InlineData("2025.10", "2025.9")]
    [InlineData("2025-10-01", "2025-09-30")]
    [InlineData("r10", "r9")]
    [InlineData("1.0.0.10", "1.0.0.9")]
    [InlineData("1.0.1", "1.0")]
    public void CompareTo_HigherVersion_IsGreater(string higher, string lower)
    {
        var higherLink = new VersionLink { Version = higher };
        var lowerLink = new VersionLink { Version = lower };

        higherLink.CompareTo(lowerLink).ShouldBeGreaterThan(0);
        lowerLink.CompareTo(higherLink).ShouldBeLessThan(0);
    }

    [Fact]
    public void CompareTo_SameVersion_IsZero()
    {
        new VersionLink { Version = "1.0.0" }.CompareTo(new VersionLink { Version = "1.0.0" }).ShouldBe(0);
    }
}
