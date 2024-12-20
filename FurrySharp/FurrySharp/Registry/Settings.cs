using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using FurrySharp.Dialogue;
using FurrySharp.Input;
using Microsoft.Xna.Framework.Input;

namespace FurrySharp.Registry;

public enum Resolution
{
    Windowed,
    Fullscreen,
}

public class Settings
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Language Language { get; set; } = Language.EN;

    public Resolution Resolution { get; set; } = Resolution.Windowed;

    public int PreferredWindowScale { get; set; } = 3;
    
    public float MusicVolumeScale { get; set; } = 1f;
    
    public float SFXVolumeScale { get; set; } = 1f;

    public bool ShowDevTools { get; set; } = false;

    [JsonInclude]
    public Dictionary<KeyFunctions, RebindableKey> KeyBindings
    {
        get => KeyInput.RebindableKeys;
        set => KeyInput.RebindableKeys = value;
    }

    public static Settings Load()
    {
        Settings settings;
        try
        {
            string save = File.ReadAllText($"{GameConstants.SavePath}Settings.json");
            settings = JsonSerializer.Deserialize<Settings>(save);
        }
        catch
        {
            settings = new Settings();
        }

        settings.ValidateKeyBindings();
        settings.Save();
        return settings;
    }

    public void Save()
    {
        File.WriteAllText($"{GameConstants.SavePath}Settings.json", JsonSerializer.Serialize(this));
    }

    private void ValidateKeyBindings()
    {
        KeyBindings ??= new();

        ValidateHelper(KeyFunctions.Up, new RebindableKey(new List<Keys>() { Keys.W, Keys.Up }));
        ValidateHelper(KeyFunctions.Down, new RebindableKey(new List<Keys>() { Keys.S, Keys.Down }));
        ValidateHelper(KeyFunctions.Left, new RebindableKey(new List<Keys>() { Keys.A, Keys.Left }));
        ValidateHelper(KeyFunctions.Right, new RebindableKey(new List<Keys>() { Keys.D, Keys.Right }));
        return;

        void ValidateHelper(KeyFunctions function, RebindableKey defaultKey)
        {
            KeyBindings.TryAdd(function, defaultKey);
        }
    }
}