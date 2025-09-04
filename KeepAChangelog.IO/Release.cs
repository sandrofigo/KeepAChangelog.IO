using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeepAChangelog.IO;

public record Release : IComparable<Release>
{
    internal const string Symbol = "## ";
    public const string UnreleasedVersionString = "Unreleased";
    public const string YankedSymbol = "[YANKED]";

    public string Version { get; set; } = UnreleasedVersionString;

    /// <summary>
    /// The release date in YYYY-MM-DD format.
    /// </summary>
    /// <remarks>
    /// If null, the release is considered "Unreleased".
    /// </remarks>
    public ReleaseDate? ReleaseDate { get; set; }

    public List<Category> Categories { get; set; } = [];

    public bool IsReleased => ReleaseDate is not null;
    public bool IsUnreleased => !IsReleased;


    /// <summary>
    /// Marks the release as yanked and appends the "[YANKED]" label after the release date.
    /// </summary>
    /// <remarks>
    /// Should be set to True for releases that have been removed from distribution (e.g. due to a critical issue or security vulnerability).
    /// </remarks>
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

    public int CompareTo(Release? other)
    {
        if (ReferenceEquals(this, other))
            return 0;

        if (other is null)
            return 1;

        if (IsUnreleased && other.IsUnreleased)
            return 0;

        if (IsUnreleased)
            return 1;

        if (other.IsUnreleased)
            return -1;

        int releaseDateComparison = Comparer<ReleaseDate?>.Default.Compare(ReleaseDate, other.ReleaseDate);
        if (releaseDateComparison != 0)
            return releaseDateComparison;

        int versionComparison = string.Compare(Version, other.Version, StringComparison.Ordinal);
        return versionComparison;
    }
}