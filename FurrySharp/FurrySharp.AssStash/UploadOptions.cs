using CommandLine;

namespace FurrySharp.AssStash;

[Verb("upload", HelpText = "Uploads ass to the cloud")]
public class UploadOptions : BaseOptions
{
    [Value(0)]
    public IEnumerable<string> SearchPatterns { get; set; }
}