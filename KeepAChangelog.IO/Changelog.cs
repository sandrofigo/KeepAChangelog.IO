using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace KeepAChangelog.IO;

internal enum ParsingContext
{
    Title,
    Description,
    ReleaseSection,
    EntryCategory,
    Entry,
    VersionLink
}

public class Changelog
{
    private const string TitleSymbol = "# ";

    private const string DefaultChangelogTitle = "Changelog";

    private const string DefaultChangelogDescription = """
                                                       All notable changes to this project will be documented in this file.

                                                       The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
                                                       and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).
                                                       """;

    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public readonly List<Release> Releases = [];
    public readonly List<VersionLink> VersionLinks = [];

    private Changelog()
    {
    }

    /// <summary>
    /// Creates an empty changelog with the default title and description and a single empty unreleased release.
    /// </summary>
    /// <returns></returns>
    public static Changelog Create()
    {
        var changelog = new Changelog
        {
            Title = DefaultChangelogTitle,
            Description = DefaultChangelogDescription
        };
        changelog.Releases.Add(new Release());
        return changelog;
    }

    public static Changelog FromFile(string changelogFilePath)
    {
        string[] lines = File.ReadAllLines(changelogFilePath);

        return Parse(lines);
    }

    private static Changelog Parse(IEnumerable<string> lines)
    {
        var changelog = new Changelog();

        var context = ParsingContext.Title;

        var description = new StringBuilder();

        foreach (string line in lines)
        {
            switch (context)
            {
                case ParsingContext.Description when IsReleaseSection(line):
                    changelog.Description = description.ToString().Trim();
                    context = ParsingContext.ReleaseSection;
                    break;
                case ParsingContext.EntryCategory when IsEntry(line):
                    context = ParsingContext.Entry;
                    break;
                case ParsingContext.Entry when IsCategory(line):
                    context = ParsingContext.EntryCategory;
                    break;
                case ParsingContext.Entry when IsReleaseSection(line):
                    context = ParsingContext.ReleaseSection;
                    break;
                case ParsingContext.Entry when IsVersionLink(line):
                    context = ParsingContext.VersionLink;
                    break;
            }

            Release lastAddedRelease;
            switch (context)
            {
                case ParsingContext.Title:
                    if (IsTitle(line))
                    {
                        changelog.Title = line.Substring(TitleSymbol.Length, line.Length - TitleSymbol.Length).Trim();
                        context = ParsingContext.Description;
                    }

                    break;
                case ParsingContext.Description:
                    description.AppendLine(line);
                    break;
                case ParsingContext.ReleaseSection:
                    string version = Regex.Match(line, @"\[(.*?)\]").Groups[1].Value.Trim();
                    Match releaseDate = Regex.Match(line, @"(\d+)-(\d+)-(\d+)");
                    bool isYanked = line.Contains(Release.YankedSymbol);

                    changelog.Releases.Add(new Release
                    {
                        Version = version,
                        ReleaseDate = releaseDate.Success ? new ReleaseDate(int.Parse(releaseDate.Groups[1].Value.Trim()), int.Parse(releaseDate.Groups[2].Value.Trim()), int.Parse(releaseDate.Groups[3].Value.Trim())) : null,
                        IsYanked = isYanked
                    });

                    context = ParsingContext.EntryCategory;

                    break;
                case ParsingContext.EntryCategory:
                    lastAddedRelease = changelog.Releases[changelog.Releases.Count - 1];
                    if (IsCategory(line))
                    {
                        string categoryName = line.Substring(Category.Symbol.Length, line.Length - Category.Symbol.Length).Trim().ToLowerInvariant();

                        if (Enum.TryParse(categoryName, true, out EntryType entryType) && changelog.Releases.Count > 0)
                        {
                            lastAddedRelease.Categories.Add(new Category
                            {
                                Type = entryType
                            });

                            context = ParsingContext.Entry;
                        }
                    }

                    break;
                case ParsingContext.Entry:
                    lastAddedRelease = changelog.Releases[changelog.Releases.Count - 1];
                    Category lastAddedCategory = lastAddedRelease.Categories[lastAddedRelease.Categories.Count - 1];
                    if (IsEntry(line) && changelog.Releases.Count > 0 && lastAddedRelease.Categories.Count > 0)
                    {
                        string entryText = line.Substring(Entry.Symbol.Length, line.Length - Entry.Symbol.Length).Trim();

                        lastAddedCategory.Entries.Add(new Entry
                        {
                            Text = entryText
                        });
                    }
                    else if (lastAddedCategory.Entries.Count > 0)
                    {
                        string trimmedLine = line.Trim();
                        if (trimmedLine.Length > 0)
                        {
                            Entry lastAddedEntry = lastAddedCategory.Entries[lastAddedCategory.Entries.Count - 1];
                            lastAddedEntry.Text += Environment.NewLine + line.Trim();
                        }
                    }

                    break;
                case ParsingContext.VersionLink:
                    if (IsVersionLink(line))
                    {
                        Match match = Regex.Match(line, @"\[(.*?)\]:\s*(.*)");

                        if (match.Success)
                        {
                            string linkVersion = match.Groups[1].Value.Trim();
                            string url = match.Groups[2].Value.Trim();

                            changelog.VersionLinks.Add(new VersionLink
                            {
                                Version = linkVersion,
                                Url = url
                            });
                        }
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return changelog;
    }

    private static bool IsTitle(string line)
    {
        return line.StartsWith(TitleSymbol);
    }

    private static bool IsReleaseSection(string line)
    {
        return line.StartsWith(Release.Symbol);
    }

    private static bool IsCategory(string line)
    {
        return line.StartsWith(Category.Symbol);
    }

    private static bool IsEntry(string line)
    {
        return line.StartsWith(Entry.Symbol);
    }

    private static bool IsVersionLink(string line)
    {
        return line.StartsWith(VersionLink.Symbol);
    }

    internal static readonly string DoubleNewLine = Environment.NewLine + Environment.NewLine;

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine($"{TitleSymbol}{Title}");
        stringBuilder.AppendLine();
        stringBuilder.AppendLine(Description);
        stringBuilder.AppendLine();

        stringBuilder.Append(string.Join(DoubleNewLine, Releases));

        if (VersionLinks.Count > 0)
            stringBuilder.Append(DoubleNewLine);

        stringBuilder.Append(string.Join(Environment.NewLine, VersionLinks));

        return stringBuilder.ToString();
    }
}