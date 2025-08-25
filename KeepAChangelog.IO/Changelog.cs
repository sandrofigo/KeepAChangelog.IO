using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public class Changelog
{
    public ChangelogTextSection Header;
    public List<ChangelogReleaseSection> Sections = new List<ChangelogReleaseSection>();

    public const string UnreleasedVersionString = "Unreleased";

    public static Changelog FromFile(string changelogFilePath)
    {
        string[] lines = File.ReadAllLines(changelogFilePath);

        return new Changelog
        {
            Header = GetHeader(lines),
            Sections = GetSections(lines)
        };
    }

    private static bool IsSectionStart(string line)
    {
        return line.StartsWith("## ");
    }

    private static bool IsSubSectionStart(string line)
    {
        return line.StartsWith("### ");
    }

    private static bool IsSubSectionEntry(string line)
    {
        return line.StartsWith("-");
    }

    private static bool IsVersionComparison(string line)
    {
        return line.StartsWith("[");
    }

    private static ChangelogTextSection GetHeader(IEnumerable<string> lines)
    {
        var header = new ChangelogTextSection();

        foreach (string l in lines)
        {
            if (IsSectionStart(l))
            {
                return header;
            }

            header.Lines.Add(l);
        }

        return header;
    }


    private static List<ChangelogReleaseSection> GetSections(IEnumerable<string> lines)
    {
        var sections = new List<ChangelogReleaseSection>();

        string currentSubSection = "";

        foreach (string l in lines)
        {
            if (IsSectionStart(l))
            {
                string currentSectionVersion = Regex.Match(l, @"\[(?<version>.*)\]").Groups["version"].Value.Trim();
                string currentSectionReleaseDate = Regex.Match(l, @"(?<release_date>\d+-\d+-\d+)").Groups["release_date"].Value.Trim();

                sections.Add(new ChangelogReleaseSection
                {
                    Version = currentSectionVersion,
                    ReleaseDate = currentSectionReleaseDate
                });

                currentSubSection = "";
            }
            else if (IsSubSectionStart(l) && sections.Count > 0)
            {
                string subSectionName = l.Replace("###", "").Trim().ToLowerInvariant();

                currentSubSection = subSectionName;
            }
            else if (IsSubSectionEntry(l) && !string.IsNullOrWhiteSpace(currentSubSection))
            {
                string entry = l.TrimStart('-').Trim();

                if (Enum.TryParse(currentSubSection, true, out EntryType entryType))
                {
                    sections[sections.Count - 1].Entries[entryType].Add(entry);
                }
            }
        }

        return sections;
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();

        if (Header != null)
            stringBuilder.AppendLine(Header.ToString());

        if (Sections.Any(s => s.Version == UnreleasedVersionString))
        {
            ChangelogReleaseSection unreleasedSection = Sections.First(s => s.Version == UnreleasedVersionString);

            if (unreleasedSection.IsEmpty())
                Sections.Remove(unreleasedSection);
        }

        foreach (ChangelogReleaseSection section in Sections)
        {
            stringBuilder.AppendLine(Sections.ToString());
        }

        return stringBuilder.ToString();
    }
}

public class ChangelogReleaseSection
{
    public string Version { get; set; }
    public string ReleaseDate { get; set; }

    public Dictionary<EntryType, List<string>> Entries { get; set; } = new Dictionary<EntryType, List<string>>
    {
        { EntryType.Added, new List<string>() },
        { EntryType.Changed, new List<string>() },
        { EntryType.Deprecated, new List<string>() },
        { EntryType.Removed, new List<string>() },
        { EntryType.Fixed, new List<string>() },
        { EntryType.Security, new List<string>() }
    };

    public bool IsEmpty()
    {
        return Entries.All(e => e.Value.Count == 0);
    }

    public override string ToString()
    {
        var lines = new List<string>();

        lines.Add($"## [{Version}]{(string.IsNullOrWhiteSpace(ReleaseDate) ? "" : $" - {ReleaseDate}")}");
        lines.Add("");

        foreach (var group in Entries)
        {
            if (group.Value.Count > 0)
            {
                lines.Add($"### {group.Key}");
                lines.Add("");
                foreach (string s in group.Value)
                    lines.Add($"- {s}");
                lines.Add("");
            }
        }

        var stringBuilder = new StringBuilder();

        foreach (string line in lines)
        {
            stringBuilder.AppendLine(line);
        }

        return stringBuilder.ToString();
    }
}

public class ChangelogTextSection
{
    public List<string> Lines { get; } = new List<string>();

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();

        foreach (string line in Lines)
        {
            stringBuilder.AppendLine(line);
        }

        return stringBuilder.ToString();
    }
}

public enum EntryType
{
    Added,
    Changed,
    Deprecated,
    Removed,
    Fixed,
    Security,
}