namespace KeepAChangelog.IO.Tests;

public class EntryTests
{
    [Theory]
    [InlineData("a\nb")]
    [InlineData("a\r\nb")]
    [InlineData("a\rb")]
    public void ToString_MultiLineText_IndentsContinuationLines(string text)
    {
        var entry = new Entry { Text = text };

        entry.ToString().ShouldBe($"- a{Environment.NewLine}  b");
    }
}
