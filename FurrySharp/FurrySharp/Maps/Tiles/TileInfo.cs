using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using FurrySharp.Resources;

namespace FurrySharp.Maps.Tiles;

public class TileInfo
{
    public string TileInfoPath { get; private set; }
    public List<CollisionData> Collisions;

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

        var srcPath = Path.Combine(info.TileInfoPath, "col.json");
        string text = File.ReadAllText(srcPath);
        info.Collisions = JsonSerializer.Deserialize<List<CollisionData>>(text);

        return info;
    }
}