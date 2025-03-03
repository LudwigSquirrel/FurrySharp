using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using FurrySharp.Drawing;
using FurrySharp.Logging;
using FurrySharp.Registry;
using FurrySharp.Utilities;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Xna.Framework;

namespace FurrySharp.Resources;

public static class ResourceManager
{
    private static Dictionary<string, Texture2D> textures = new();
    private static Dictionary<string, CatSpline> splines = new();
    private static Dictionary<string, string> music = new();
    private static Dictionary<string, string> ambience = new();
    private static Dictionary<string, string> map = new();
    private static Dictionary<string, string> tilemaps = new();

    public static string BaseDir;

    public static bool LoadContentManagerResources(ContentManager contentManager)
    {
        LoadTextures(contentManager);
        return true;
    }

    public static bool LoadOtherResources()
    {
        LoadMusic();
        LoadAmbience();
        LoadMaps();
        LoadTileMaps();
        LoadSplines();
        return true;
    }

    public static Texture2D GetTexture(string textureName, bool forceCorrectTexture = false, bool allowUnknown = false)
    {
        if (!forceCorrectTexture && GlobalState.GameMode != GameMode.Normal)
        {
            return textures.Values.ElementAt(GlobalState.RNG.Next(textures.Count));
        }

        if (!textures.ContainsKey(textureName))
        {
            if (!allowUnknown)
            {
                DebugLogger.AddWarning($"Texture file called {textureName}.png not found!");
            }

            return null;
        }

        return textures[textureName];
    }

    public static string GetMusicPath(string musicName)
    {
        if (music == null)
        {
            return null;
        }

        if (!music.ContainsKey(musicName))
        {
            DebugLogger.AddWarning($"Music file called {musicName}.ogg not found!");
            return null;
        }

        return music[musicName];
    }

    public static string GetAmbiencePath(string ambienceName)
    {
        if (ambience == null)
        {
            return null;
        }

        if (!ambience.ContainsKey(ambienceName))
        {
            DebugLogger.AddWarning($"Ambience file called {ambienceName}.ogg not found!");
            return null;
        }

        return ambience[ambienceName];
    }

    public static string GetMapPath(string mapName)
    {
        if (!map.ContainsKey(mapName))
        {
            DebugLogger.AddWarning($"Map directory called {mapName} not found!");
            return null;
        }

        return map[mapName];
    }
    
    public static string GetTileMapPath(string tileMapName)
    {
        if (!tilemaps.ContainsKey(tileMapName))
        {
            DebugLogger.AddWarning($"Tilemap directory called {tileMapName} not found!");
            return null;
        }

        return tilemaps[tileMapName];
    }
    
    public static CatSpline GetSpline(string splineName)
    {
        if (!splines.ContainsKey(splineName))
        {
            DebugLogger.AddWarning($"Spline file called {splineName}.json not found!");
            return null;
        }

        return splines[splineName];
    }
    
    public static Dictionary<string, CatSpline>.KeyCollection GetSplineKeys()
    {
        return splines.Keys;
    }

    public static bool UnloadResources()
    {
        UnloadTextures();
        return true;
    }

    private static void LoadMusic()
    {
        foreach (FileInfo file in GetFiles("bgm"))
        {
            string key = Path.GetFileNameWithoutExtension(file.Name);

            music[key] = file.FullName;
        }
    }

    private static void LoadAmbience()
    {
        foreach (FileInfo file in GetFiles("ambience"))
        {
            string key = Path.GetFileNameWithoutExtension(file.Name);

            ambience[key] = file.FullName;
        }
    }

    private static void LoadMaps()
    {
        var mapDirectories = Directory.GetDirectories(Path.Combine(BaseDir, "Content", "maps"));
        foreach (var mapDirectory in mapDirectories)
        {
            var mapName = new DirectoryInfo(mapDirectory).Name;
            map[mapName!] = mapDirectory;
        }
    }

    private static void LoadTileMaps()
    {
        var mapDirectories = Directory.GetDirectories(Path.Combine(BaseDir, "Content", "tilemapdata"));
        foreach (var mapDirectory in mapDirectories)
        {
            var mapName = new DirectoryInfo(mapDirectory).Name;
            tilemaps[mapName!] = mapDirectory;
        }
    }

    private static void LoadTextures(ContentManager content)
    {
        foreach (FileInfo file in GetFiles("textures"))
        {
            string key = Path.GetFileNameWithoutExtension(file.Name);

            try
            {
                textures[key] = content.Load<Texture2D>(GetFolderTree(file) + key);
            }
            catch (Exception)
            {
                textures[key] = Texture2D.FromFile(SpriteDrawer.SpriteBatch.GraphicsDevice, file.FullName);
                DebugLogger.AddError($"Failed to load texture: {key}");
            }
        }
    }

    private static void LoadSplines()
    {
        foreach (FileInfo file in GetFiles("splines"))
        {
            string key = Path.GetFileNameWithoutExtension(file.Name);
            try
            {
                string splineData = File.ReadAllText(file.FullName);
                splines[key] = JsonSerializer.Deserialize<CatSpline>(splineData, new JsonSerializerOptions()
                {
                    IncludeFields = true,
                });
            }
            catch (Exception)
            {
                DebugLogger.AddError($"Failed to load spline: {key}");
            }
        }
    }

    private static void UnloadTextures()
    {
        foreach (var texture in textures)
        {
            texture.Value.Dispose();
        }
    }

    private static IEnumerable<FileInfo> GetFiles(string dirName)
    {
        Matcher matcher = new();
        matcher.AddInclude($"./Content/{dirName}/**");
        matcher.AddExclude("./Content/**/old/");
        matcher.AddInclude($"./Mods/*/Content/{dirName}/**");

        return matcher.GetResultsInFullPath(BaseDir).Select(s => new FileInfo(s));
    }

    private static string GetFolderTree(FileInfo file)
    {
        string path = "";
        DirectoryInfo curFolder = file.Directory;

        do
        {
            path = curFolder.Name + "/" + path;
            curFolder = curFolder.Parent;
        } while (curFolder.Name != "Content");

        return path;
    }
}