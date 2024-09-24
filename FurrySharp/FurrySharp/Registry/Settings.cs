using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using FurrySharp.Dialogue;

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

    public int Scale { get; set; } = 3;

    public static Settings Load()
    {
        try
        {
            string save = File.ReadAllText($"{GameConstants.SavePath}Settings.json");
            return JsonSerializer.Deserialize<Settings>(save);
        }
        catch
        {
            return new();
        }
    }

    public void Save()
    {
        File.WriteAllText($"{GameConstants.SavePath}Settings.json", JsonSerializer.Serialize(this));
    }
}