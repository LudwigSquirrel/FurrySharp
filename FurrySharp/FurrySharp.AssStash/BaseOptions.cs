using CommandLine;

namespace FurrySharp.AssStash;

public class BaseOptions
{
    [Option('v', "verbose", Default = false, HelpText = "Logs more output")]
    public bool Verbose { get; set; }

    [Option('d', "dry", Default = false, HelpText = "Describes what would happen if the command was executed, instead of actually doing it.")]
    public bool DryRun { get; set; }
}