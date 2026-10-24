using System;
using System.Text.RegularExpressions;

namespace KeepAChangelog.IO;

/// <summary>
/// Compares version strings. Uses semantic versioning precedence when both versions are semantic versions,
/// otherwise falls back to a natural sort, so changelogs that don't follow semantic versioning still sort sensibly.
/// </summary>
internal static class VersionComparer
{
    private static readonly Regex SemanticVersionRegex = new(@"^v?(\d+)\.(\d+)\.(\d+)(?:-([0-9A-Za-z.-]+))?(?:\+[0-9A-Za-z.-]+)?$");

    private static readonly Regex NaturalTokenRegex = new(@"\d+|\D+");

    public static int Compare(string x, string y)
    {
        Match xMatch = SemanticVersionRegex.Match(x);
        Match yMatch = SemanticVersionRegex.Match(y);

        int comparison = xMatch.Success && yMatch.Success
            ? CompareSemanticVersions(xMatch, yMatch)
            : CompareNatural(x, y);

        // Keep the order deterministic for versions with equal precedence, e.g. differing only in build metadata
        return comparison != 0 ? comparison : string.CompareOrdinal(x, y);
    }

    private static int CompareSemanticVersions(Match x, Match y)
    {
        for (int group = 1; group <= 3; group++)
        {
            int comparison = CompareNumbers(x.Groups[group].Value, y.Groups[group].Value);
            if (comparison != 0)
                return comparison;
        }

        Group xPreRelease = x.Groups[4];
        Group yPreRelease = y.Groups[4];

        // A version without pre-release is greater than one with it (1.0.0 > 1.0.0-beta)
        if (!xPreRelease.Success || !yPreRelease.Success)
            return yPreRelease.Success.CompareTo(xPreRelease.Success);

        string[] xIdentifiers = xPreRelease.Value.Split('.');
        string[] yIdentifiers = yPreRelease.Value.Split('.');

        for (int i = 0; i < Math.Min(xIdentifiers.Length, yIdentifiers.Length); i++)
        {
            int comparison = CompareTokens(xIdentifiers[i], yIdentifiers[i]);
            if (comparison != 0)
                return comparison;
        }

        return xIdentifiers.Length.CompareTo(yIdentifiers.Length);
    }

    private static int CompareNatural(string x, string y)
    {
        MatchCollection xTokens = NaturalTokenRegex.Matches(x);
        MatchCollection yTokens = NaturalTokenRegex.Matches(y);

        for (int i = 0; i < Math.Min(xTokens.Count, yTokens.Count); i++)
        {
            int comparison = CompareTokens(xTokens[i].Value, yTokens[i].Value);
            if (comparison != 0)
                return comparison;
        }

        return xTokens.Count.CompareTo(yTokens.Count);
    }

    /// <summary>
    /// Numeric tokens compare numerically and sort below non-numeric tokens, which compare ordinally.
    /// </summary>
    private static int CompareTokens(string x, string y)
    {
        bool xIsNumber = IsNumber(x);
        bool yIsNumber = IsNumber(y);

        if (xIsNumber && yIsNumber)
            return CompareNumbers(x, y);

        if (xIsNumber != yIsNumber)
            return xIsNumber ? -1 : 1;

        return string.CompareOrdinal(x, y);
    }

    /// <summary>
    /// Compares digit strings of any length without parsing them, so large numbers can't overflow.
    /// </summary>
    private static int CompareNumbers(string x, string y)
    {
        x = x.TrimStart('0');
        y = y.TrimStart('0');

        int lengthComparison = x.Length.CompareTo(y.Length);
        return lengthComparison != 0 ? lengthComparison : string.CompareOrdinal(x, y);
    }

    private static bool IsNumber(string value)
    {
        if (value.Length == 0)
            return false;

        foreach (char c in value)
        {
            if (c is < '0' or > '9')
                return false;
        }

        return true;
    }
}