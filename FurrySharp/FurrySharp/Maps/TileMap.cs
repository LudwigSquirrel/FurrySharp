namespace FurrySharp.Maps;

public class TileMap
{
    private readonly int[,] tiles;

    public readonly int Width;
    public readonly int Height;

    public TileMap(int[,] tiles)
    {
        this.tiles = tiles;
        
        Width = tiles.GetLength(0);
        Height = tiles.GetLength(1);
    }

    public int GetTile(int x, int y)
    {
        if (x >= Width || y >= Height || x < 0 || y < 0)
        {
            return 0;
        }
        return tiles[x, y];
    }

    public int[,] GetMap() => tiles;
}