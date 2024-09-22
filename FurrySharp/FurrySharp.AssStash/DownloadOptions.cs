using CommandLine;

namespace FurrySharp.AssStash;

[Verb("download", HelpText = "Downloads ass locally")]
public class DownloadOptions : BaseOptions
{
    [Value(0)]
    public IEnumerable<string> SearchRegexes { get; set; }
}