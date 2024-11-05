using CommandLine;

namespace FurrySharp.AssStash;

[Verb("config", HelpText = "Configs this application. Configuration is stored in './options.json'.")]
public class ConfigOptions
{
    [Option('c', "contentPath", Default = null, HelpText = "Sets the content path.")]
    public string ContentPath { get; set; }

    [Option('f', "folderId", Default = null, HelpText = "Sets the folder id for uploading.")]
    public string FolderId { get; set; }

    [Option('s', "secretsPath", Default = null, HelpText = "Sets the secret folder path.")]
    public string SecretsPath { get; set; }
}