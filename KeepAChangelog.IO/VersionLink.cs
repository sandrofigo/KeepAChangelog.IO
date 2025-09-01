namespace KeepAChangelog.IO;

public record VersionLink
{
    internal const string Symbol = "[";

    public string Version { get; set; } = "";
    public string Url { get; set; } = "";

    public override string ToString()
    {
        return $"[{Version}]: {Url}";
    }
}