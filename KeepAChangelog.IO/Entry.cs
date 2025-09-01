using System;
using System.Text;

namespace KeepAChangelog.IO;

public record Entry
{
    internal const string Symbol = "- ";

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