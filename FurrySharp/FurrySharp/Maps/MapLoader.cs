using System.IO;
using FurrySharp.Resources;
using FurrySharp.Utilities;

namespace FurrySharp.Maps;

public static class MapLoader
{
    public static MapInfo LoadMap(string name)
    {
        var path = ResourceManager.GetMapPath(name);
        
        MapInfo map = MapInfo.FromDir(path);

        return map;
    }
}