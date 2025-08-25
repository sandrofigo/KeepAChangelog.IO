using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeepAChangelog.IO;

public class ChangelogReleaseSection
{
    public string Version { get; set; }
    public string ReleaseDate { get; set; }

    public Dictionary<EntryType, List<string>> Entries { get; set; } = new()
    {
        { EntryType.Added, [] },
        { EntryType.Changed, [] },
        { EntryType.Deprecated, [] },
        { EntryType.Removed, [] },
        { EntryType.Fixed, [] },
        { EntryType.Security, [] }
    };

    public bool IsEmpty()
    {
        return Entries.All(e => e.Value.Count == 0);
    }

    public override string ToString()
    {
        var lines = new List<string>
        {
            $"## [{Version}]{(string.IsNullOrWhiteSpace(ReleaseDate) ? "" : $" - {ReleaseDate}")}",
            ""
        };

        foreach (var group in Entries)
        {
            if (group.Value.Count <= 0)
                continue;
            
            lines.Add($"### {group.Key}");
            lines.Add("");
            foreach (string s in group.Value)
                lines.Add($"- {s}");
            lines.Add("");
        }

        var stringBuilder = new StringBuilder();

        foreach (string line in lines)
        {
            stringBuilder.AppendLine(line);
        }

        return stringBuilder.ToString();
    }
}