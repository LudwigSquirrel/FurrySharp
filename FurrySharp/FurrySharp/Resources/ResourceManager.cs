using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FurrySharp.Drawing;
using FurrySharp.Logging;
using FurrySharp.Registry;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Extensions.FileSystemGlobbing;

namespace FurrySharp.Resources;

public static class ResourceManager
{
    private static Dictionary<string, Texture2D> textures = new();
    
    public static string BaseDir;

    public static bool LoadResources(ContentManager contentManager)
    {
        LoadTextures(contentManager);

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
    
    public static bool UnloadResources()
    {
        UnloadTextures();
        return true;
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