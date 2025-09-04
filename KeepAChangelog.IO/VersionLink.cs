using System;

namespace KeepAChangelog.IO;

public record VersionLink : IComparable<VersionLink>
{
    internal const string Symbol = "[";

    public string Version { get; set; } = "";
    public string Url { get; set; } = "";

    public override string ToString()
    {
        return $"[{Version}]: {Url}";
    }

    public int CompareTo(VersionLink? other)
    {
        if (ReferenceEquals(this, other))
            return 0;

        if (other is null)
            return 1;

        return string.Compare(Version, other.Version, StringComparison.Ordinal);
    }
}