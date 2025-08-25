using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace KeepAChangelog.IO;

public class Changelog
{
    public ChangelogTextSection Header;
    public List<ChangelogReleaseSection> Sections = [];

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
            stringBuilder.AppendLine(section.ToString());
        }

        return stringBuilder.ToString();
    }
}