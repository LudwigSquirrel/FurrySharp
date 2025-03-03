using System.Collections.Generic;
using FurrySharp.Maps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FurrySharp.Drawing;

public class TrailUnit
{
    public Vector2 Position;
    public Color Color = Color.White;
    public float Scale = 1f;
    public float Rotation = 0f;
    public int Index = 0;
}

public class Trail
{
    public List<TrailUnit> Units = new();

    public Spritesheet Spritesheet;

    public void AddDefaultUnits(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Units.Add(new TrailUnit());
        }
    }

    public void DrawTrail(MapInfo mapInfo)
    {
        foreach (var unit in Units)
        {
            var rect = Spritesheet.GetRect(unit.Index);
            float z = GetTrailUnitZ(mapInfo, unit);
            SpriteDrawer.SpriteBatch.Draw(
                Spritesheet.Tex,
                unit.Position,
                rect,
                unit.Color,
                unit.Rotation,
                new Vector2(rect.Width / 2f, rect.Height / 2f),
                unit.Scale,
                SpriteEffects.None,
                z);
        }
    }

    private static float GetTrailUnitZ(MapInfo map, TrailUnit unit)
    {
        return DrawingUtilities.GetDrawingZ(DrawOrder.Entities, unit.Position.Y, map.HeightInPixels);
    }
}