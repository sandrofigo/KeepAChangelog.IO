using System.Collections.Generic;
using System.Text;

namespace KeepAChangelog.IO;

public class ChangelogTextSection
{
    public List<string> Lines { get; } = [];

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