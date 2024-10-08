using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using FurrySharp.Audio;
using FurrySharp.Drawing;
using FurrySharp.Logging;
using FurrySharp.Resources;
using FurrySharp.Utilities;
using Microsoft.Xna.Framework;
using static FurrySharp.Registry.GameConstants;

namespace FurrySharp.Maps;

public enum MapLayer
{
    BG,
}

public class MapInfo
{
    public string MapPath { get; private set; }
    public bool Critical { get; private set; }
    
    public Spritesheet TileMap { get; private set; }

    public List<TileMap> TileMaps { get; private set; }

    public MapSettings Settings { get; private set; }

    public static MapInfo FromDir(string path, bool critical = false)
    {
        var map = new MapInfo()
        {
            MapPath = path,
            Critical = critical,
        };

        // Foreach enum value in MapLayer, load the corresponding tilemap

        var values = Enum.GetValues(typeof(MapLayer));
        map.TileMaps = new List<TileMap>(values.Length);
        foreach (MapLayer layer in values)
        {
            map.TileMaps.Add(new TileMap(map.LoadBmp(layer.ToString())));
        }
        
        map.Settings = map.LoadJson<MapSettings>("settings") ?? new MapSettings();

        map.TileMap = new Spritesheet(ResourceManager.GetTexture($"{map.Settings.TileMap}_tilemap"), TILE_SIZE, TILE_SIZE);

        return map;
    }

    public int[,] LoadBmp(string name)
    {
        try
        {
            var srcPath = Path.Combine(MapPath, $"{name}.bmp");
            return BitmapLoader.LoadPixelData(srcPath);
        }
        catch (Exception e)
        {
            DebugLogger.AddException(e);
            if (Critical)
            {
                DebugLogger.AddCritical($"Unable to load file {name}.bmp from {MapPath}");
            }
        }

        return new int[,] { };
    }

    public T LoadJson<T>(string json)
    {
        try
        {
            var srcPath = Path.Combine(MapPath, $"{json}.json");
            using var stream = new StreamReader(srcPath);
            return JsonSerializer.Deserialize<T>(stream.ReadToEnd());
        }
        catch (Exception e)
        {
            DebugLogger.AddException(e);
            if (Critical)
            {
                DebugLogger.AddCritical($"Unable to load file {json}.json from {MapPath}");
            }
        }

        return default;
    }

    public void DrawLayer(Rectangle bounds, int layer, DrawOrder order, bool ignoreEmpty)
    {
        // todo: implement
        float z = DrawingUtilities.GetDrawingZ(order);

        Point tl = ToMapLoc(new(bounds.X, bounds.Y));
        Point br = ToMapLoc(new(bounds.Right, bounds.Bottom));

        for (int y = tl.Y - 1; y < br.Y + 1; y++)
        {
            for (int x = tl.X - 1; x < br.X + 1; x++)
            {
                int tile = GetTile(layer, x, y);
                
                if (tile == 0 && ignoreEmpty)
                {
                    continue;
                }

                Rectangle source = TileMap.GetRect(tile);
                Point loc = TileToWorld(x, y);
                Rectangle dest = new(loc.X, loc.Y, TILE_SIZE, TILE_SIZE);

                SpriteDrawer.DrawSprite(TileMap.Tex, dest, source, z: z);
            }
        }
    }

    public Point ToMapLoc(Point screenLoc)
    {
        return new(screenLoc.X / TILE_SIZE, screenLoc.Y / TILE_SIZE);
    }
    
    public Point TileToWorld(int x, int y)
    {
        return new(x * TILE_SIZE, y * TILE_SIZE);
    }
    
    public int GetTile(int layer, int x, int y)
    {
        return TileMaps[layer].GetTile(x, y);
    }
}