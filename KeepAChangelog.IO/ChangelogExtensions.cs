using System.IO;

namespace KeepAChangelog.IO;

public static partial class ChangelogExtensions
{
    /// <summary>
    /// Saves the changelog to a file at the specified path.
    /// </summary>
    public static void ToFile(this Changelog changelog, string filePath)
    {
        File.WriteAllText(filePath, changelog.ToString());
    }
}