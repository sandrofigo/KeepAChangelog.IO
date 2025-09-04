using System;
using System.Text;

namespace KeepAChangelog.IO;

/// <summary>
/// An entry represents a single change under a category.
/// </summary>
public record Entry
{
    internal const string Symbol = "- ";

    /// <remarks>
    /// The text can span multiple lines by using newline characters.
    /// </remarks>
    public string Text { get; set; } = "";

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();

        string[] lines = Text.Split(["\r\n", "\n", "\r"], StringSplitOptions.None);
        string formattedText = string.Join($"{Environment.NewLine}  ", lines);

        stringBuilder.Append($"{Symbol}{formattedText}");

        return stringBuilder.ToString();
    }
}