using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using FurrySharp.Entities.Base;
using FurrySharp.Resources;
using static FurrySharp.Registry.GameConstants;

namespace FurrySharp.Maps.Tiles;

public class TileInfo
{
    public string TileInfoPath { get; private set; }
    public List<TileData> Data = new(SUPPORTED_TILE_COUNT);

    public static TileInfo FromResources(string name)
    {
        var path = ResourceManager.GetTileMapPath(name);

        return FromDir(path);
    }

    public static TileInfo FromDir(string path)
    {
        var info = new TileInfo()
        {
            TileInfoPath = path,
        };

        for (int i = 0; i < SUPPORTED_TILE_COUNT; i++)
        {
            info.Data.Add(new TileData());
        }

        var srcPath = Path.Combine(info.TileInfoPath, "col.json");
        string text = File.ReadAllText(srcPath);
        var col = JsonSerializer.Deserialize<List<CollisionData>>(text);
        foreach (var c in col)
        {
            for (int i = c.Start; i <= (c.End ?? c.Start); i++)
            {
                info.Data[i].CollisionDirection = c.Allowed;
            }
        }

        return info;
    }

    public TileData GetData(int tileIdx)
    {
        return Data[tileIdx];
    }

    public class TileData
    {
        public Touching CollisionDirection { get; set; } = Touching.ANY;
    }
}