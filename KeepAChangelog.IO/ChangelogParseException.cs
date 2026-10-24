using System;

namespace KeepAChangelog.IO;

/// <summary>
/// The exception that is thrown when a changelog cannot be parsed.
/// </summary>
public class ChangelogParseException(string message, int lineNumber) : FormatException($"Line {lineNumber}: {message}")
{
    /// <summary>
    /// The 1-based line number at which parsing failed.
    /// </summary>
    public int LineNumber { get; } = lineNumber;
}
