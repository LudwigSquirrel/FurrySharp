using System;
using System.Collections.Generic;
using FurrySharp.Logging;

namespace FurrySharp.Resources;

public class ResourceInstanceAutoSaver
{
    public static string AutoSaveDir;

    public string ResourceName { get; set; }
    public string ResourceExt { get; set; } = ".json";
    public Func<string> SaveAction { get; set; }
    public DateTime NextSaveDateTime { get; set; }
    public TimeSpan SaveInterval { get; set; }
    public int SaveCount { get; set; } = 0;

    public List<string> AutoSaveFiles { get; set; } = new();

    public ResourceInstanceAutoSaver(Func<string> saveAction, TimeSpan saveInterval)
    {
        SaveAction = saveAction;
        SaveInterval = saveInterval;
        NextSaveDateTime = DateTime.Now + saveInterval;
    }

    public void DoAutoSave()
    {
        if (DateTime.Now < NextSaveDateTime)
        {
            return;
        }

        PerformSave();

        NextSaveDateTime = DateTime.Now + SaveInterval;
    }

    public void ForceSave()
    {
        NextSaveDateTime = DateTime.Now - SaveInterval;
        DoAutoSave();
    }

    private string CreateAutoSavePath()
    {
        return $"{AutoSaveDir}/{ResourceName}{SaveCount++: 00}{ResourceExt}";
    }

    public void PerformSave()
    {
        string path = CreateAutoSavePath();
        string data = SaveAction.Invoke();

        try
        {
            System.IO.File.WriteAllText(path, data);
        }
        catch (Exception ex)
        {
            DebugLogger.AddCritical($"Failed to save {ResourceName} to {path}");
            DebugLogger.AddException(ex);
            DebugLogger.AddInfo(data);
            throw;
        }

        AutoSaveFiles.Add(path);
    }
}