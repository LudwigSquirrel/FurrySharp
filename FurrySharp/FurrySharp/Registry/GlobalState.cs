using System;
using FurrySharp.Dialogue;

namespace FurrySharp.Registry;

public enum GameMode
{
    Normal,
    TestMode,
}

public static class GlobalState
{
    public static GameMode GameMode { get; set; } = GameMode.Normal;
    public static Random RNG = new("UwU".GetHashCode());
    public static readonly Settings Settings = Settings.Load();
    public static Language CurrentLanguage => Settings.Language;
    public static bool DrawTextWriterAreas = false;
    public static bool DrawHitBoxes = false;
    public static bool DrawHitRadii = false;
    public static bool ShowFPS = false;
    public static bool ShowTerminal = false;
    public static bool ShowDevTools
    {
        get => Settings.ShowDevTools;
        set => Settings.ShowDevTools = value;
    }

    public static bool ResolutionDirty = false;
    
    public static void SetResolution(Resolution resolution)
    {
        Settings.Resolution = resolution;
        ResolutionDirty = true;
    }
    
    public static Resolution GetResolution()
    {
        return Settings.Resolution;
    }
}