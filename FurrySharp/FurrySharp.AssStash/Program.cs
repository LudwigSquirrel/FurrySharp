using System.Text.RegularExpressions;
using CommandLine;
using FurrySharp.AssStash;
using System.Text.Json;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

public static class Program
{
    public static bool VerboseMode;
    public static bool DryRunMode;

    public static string[] SCOPES = { DriveService.Scope.DriveFile };
    public static ConfigOptions Config = GetConfig();

    public static int Main(string[] args)
    {
        return Parser.Default.ParseArguments<UploadOptions, DownloadOptions, ConfigOptions>(args)
            .MapResult(
                (UploadOptions options) => RunUploadAndReturnExitCode(options),
                (DownloadOptions options) => RunDownloadAndReturnExitCode(options),
                (ConfigOptions options) => RunConfigAndReturnExitCode(options),
                errors => 1);
    }

    private static int RunConfigAndReturnExitCode(ConfigOptions options)
    {
        // Update options.json with the new values (if they exist).
        if (options.ContentPath != null)
        {
            Config.ContentPath = options.ContentPath;
        }

        if (options.FolderId != null)
        {
            Config.FolderId = options.FolderId;
        }

        if (options.SecretsPath != null)
        {
            Config.SecretsPath = options.SecretsPath;
        }

        // Write options.json.
        File.WriteAllText("options.json", JsonSerializer.Serialize(Config));

        return 0;
    }

    private static int RunUploadAndReturnExitCode(object opts)
    {
        UploadOptions upload = (UploadOptions)opts;
        VerboseMode = upload.Verbose;
        DryRunMode = upload.DryRun;
        VerboseOutput($"Attempting to upload some ass!");

        // Find file paths that match the search patterns under the content folder.
        if (Config.ContentPath == null)
        {
            VerboseOutput("Please set the ContentPath using 'config -c <path>'");
            return 1;
        }

        HashSet<string> matches = new();
        var contentPath = Path.GetFullPath(Config.ContentPath);
        VerboseOutput($"ContentPath: {contentPath}");
        foreach (var pattern in upload.SearchPatterns)
        {
            VerboseOutput($"Begin searching with pattern {pattern}\n");
            foreach (var file in Directory.GetFiles(contentPath, pattern, SearchOption.AllDirectories))
            {
                VerboseOutput(matches.Add(file) ? $"ADD: {file}" : $"EXISTS: {file}");
            }

            VerboseOutput($"\nEnd searching with pattern {pattern}\n");
        }

        // Remove the files that contain keywords in the path.
        VerboseOutput("Removing files that contain keywords in the path.");
        var keywords = new string[] { "Content.mgcb", "obj", "bin" };
        foreach (var match in matches)
        {
            if (keywords.Any(keyword => match.Split('\\').Contains(keyword)))
            {
                VerboseOutput($"REMOVED: {match}");
                matches.Remove(match);
            }
        }

        if (VerboseMode)
        {
            VerboseOutput($"Found {matches.Count} files to upload.");
            foreach (var match in matches)
            {
                VerboseOutput(match);
            }
        }

        // Upload the files to the cloud.
        VerboseOutput(DryRunMode ? "Dry run mode enabled. No files will be uploaded." : "Uploading files...");
        foreach (var match in matches)
        {
            UploadMedia(match).Wait();
        }

        return 0;
    }

    private static int RunDownloadAndReturnExitCode(object opts)
    {
        DownloadOptions download = (DownloadOptions)opts;
        VerboseMode = download.Verbose;
        DryRunMode = download.DryRun;
        VerboseOutput($"Attempting to download some ass!");
        
        // List all files in the target folder.
        if (Config.FolderId == null)
        {
            VerboseOutput("Please set the FolderId using 'config -f <id>'");
            return 1;
        }
        
        var credential = GetCredentials().Result;
        var service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential
        });
        
        var listRequest = service.Files.List();
        listRequest.Q = $"'{Config.FolderId}' in parents and trashed = false";
        listRequest.Fields = "files(id, name)";
        var listResponse = listRequest.Execute();
        
        VerboseOutput($"Found {listResponse.Files.Count} files in the target folder.");
        
        // Check if the files match a search regex.
        var regexes = download.SearchRegexes.Select(s => new Regex(s)).ToList();
        List<Google.Apis.Drive.v3.Data.File> matches = new();
        foreach (var file in listResponse.Files)
        {
            var localName = ConvertRemotePathToLocalPath(file.Name);
            foreach (var regex in regexes)
            {
                if (regex.IsMatch(localName))
                {
                    matches.Add(file);
                    VerboseOutput($"MATCH: {localName}");
                }
            }
        }
        
        if (VerboseMode)
        {
            VerboseOutput($"Found {matches.Count} files to download.");
            foreach (var match in matches)
            {
                VerboseOutput(ConvertRemotePathToLocalPath(match.Name));
            }
        }
        
        // Download the files to the local content folder.
        VerboseOutput(DryRunMode ? "Dry run mode enabled. No files will be downloaded." : "Downloading files...");
        foreach (var match in matches)
        {
            var localPath = ConvertRemotePathToLocalPath(match.Name);
            var request = service.Files.Get(match.Id);
            if (!DryRunMode)
            {
                using var stream = new FileStream(localPath, FileMode.Create, FileAccess.Write);
                request.MediaDownloader.ProgressChanged += (progress) => VerboseOutput($"{localPath} {progress.Status} {progress.BytesDownloaded}\r");
                request.Download(stream);
                VerboseOutput("\n");
            }
        }
        return 0;
    }

    public static void VerboseOutput(string message)
    {
        if (VerboseMode)
        {
            Console.WriteLine(message);
        }
    }

    public static ConfigOptions GetConfig()
    {
        // Create options.json if it doesn't exist.
        if (!File.Exists("options.json"))
        {
            File.WriteAllText("options.json", "{}");
        }

        // Read options.json.
        string json = File.ReadAllText("options.json");
        ConfigOptions config = JsonSerializer.Deserialize<ConfigOptions>(json) ?? new ConfigOptions();

        return config;
    }

    public static async Task<UserCredential> GetCredentials()
    {
        string secretsPath;
        if (Config.SecretsPath != null)
        {
            secretsPath = Path.GetFullPath(Config.SecretsPath);
            GoogleWebAuthorizationBroker.Folder = secretsPath;
        }
        else
        {
            VerboseOutput("Please set the SecretsPath using 'config -s <path>'");
            return null;
        }

        var clientSecret = Path.Combine(secretsPath, "client_secret.json");
        using var stream = new FileStream(clientSecret, FileMode.Open, FileAccess.Read);
        UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            GoogleClientSecrets.FromFile(clientSecret).Secrets,
            SCOPES,
            "user",
            CancellationToken.None
        );

        return credential;
    }

    public static async Task UploadMedia(string localFile)
    {
        if (Config.FolderId == null)
        {
            VerboseOutput("Please set the FolderId using 'config -f <id>'");
            return;
        }

        var credential = GetCredentials().Result;
        var service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential
        });

        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
        {
            Name = ConvertLocalPathToRemotePath(localFile),
            Parents = new List<string> { Config.FolderId }
        };
        var mimeType = MimeMapping.MimeUtility.GetMimeMapping(localFile) ?? "text/plain"; // hoping this doesn't bite me in the ***

        // Search for the file by name in the target folder.
        var searchRequest = service.Files.List();
        searchRequest.Q = $"name = '{fileMetadata.Name}' and '{Config.FolderId}' in parents and trashed = false";
        searchRequest.Fields = "files(id, name)";
        var searchResponse = await searchRequest.ExecuteAsync();

        VerboseOutput($"Found {searchResponse.Files.Count} files with the name {fileMetadata.Name} in the target folder.");
        if (searchResponse.Files.Count > 0)
        {
            // File exists, update it.
            fileMetadata.Parents = null;
            var existingFile = searchResponse.Files[0];
            VerboseOutput($"Updating {localFile} with ID: {existingFile.Id}");
            var updateRequest = service.Files.Update(fileMetadata, existingFile.Id, new FileStream(localFile, FileMode.Open), mimeType);
            updateRequest.Fields = "id";
            updateRequest.ResponseReceived += (file) => VerboseOutput($"Updated {localFile} with ID: {file.Id}");
            if (!DryRunMode)
            {
                var progress = updateRequest.Upload();
                VerboseOutput($"{progress.Status} {progress.BytesSent} {progress.Exception}");
            }
        }
        else
        {
            // Upload the file.
            var createRequest = service.Files.Create(fileMetadata, new FileStream(localFile, FileMode.Open), mimeType);
            createRequest.Fields = "id";
            createRequest.ResponseReceived += (file) => VerboseOutput($"Uploaded {localFile} with ID: {file.Id}");
            if (!DryRunMode)
            {
                var progress = createRequest.Upload();
                VerboseOutput($"{progress.Status} {progress.BytesSent} {progress.Exception}");
            }
        }
    }

    public static string ConvertLocalPathToRemotePath(string localPath)
    {
        // Get the relative path from the content folder.
        var relativePath = localPath.Replace(Config.ContentPath + '\\', "");
        return relativePath.Replace("\\", "--");
    }
    
    public static string ConvertRemotePathToLocalPath(string remotePath)
    {
        // Get the relative path from the content folder.
        var relativePath = remotePath.Replace("--", "\\");
        return Path.Combine(Config.ContentPath, relativePath);
    }
}