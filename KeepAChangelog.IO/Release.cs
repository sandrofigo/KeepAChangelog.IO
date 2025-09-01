using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeepAChangelog.IO;

public record Release
{
    internal const string Symbol = "## ";
    public const string UnreleasedVersionString = "Unreleased";
    public const string YankedSymbol = "[YANKED]";

    public string Version { get; set; } = UnreleasedVersionString;
    public ReleaseDate? ReleaseDate { get; set; }
    public List<Category> Categories { get; set; } = [];

    public bool IsReleased => ReleaseDate is not null;
    public bool IsUnreleased => !IsReleased;
    public bool IsYanked { get; set; }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.Append($"{Symbol}[{Version}]{(IsReleased ? $" - {ReleaseDate}" : "")}{(IsYanked ? $" {YankedSymbol}" : "")}");

        if (Categories.Count > 0)
            stringBuilder.Append(Changelog.DoubleNewLine);

        stringBuilder.Append(string.Join(Changelog.DoubleNewLine, Categories.OrderBy(category => category.Type)));

        return stringBuilder.ToString();
    }
}