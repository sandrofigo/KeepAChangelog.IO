using System;
using System.Collections.Generic;
using System.Text;

namespace KeepAChangelog.IO;

public record Category // TODO-SFIGO: indexer mimicking dictionary to insert/update by type
{
    internal const string Symbol = "### ";

    public CategoryType Type { get; set; }

    public List<Entry> Entries { get; set; } = [];

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.Append($"{Symbol}{Type}");

        if (Entries.Count > 0)
            stringBuilder.Append(Changelog.DoubleNewLine);

        stringBuilder.Append(string.Join(Environment.NewLine, Entries));

        return stringBuilder.ToString();
    }
}