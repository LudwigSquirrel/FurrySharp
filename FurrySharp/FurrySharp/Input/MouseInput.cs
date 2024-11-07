using FurrySharp.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using static FurrySharp.Registry.GameConstants;

namespace FurrySharp.Input;

public static class MouseInput
{
    public static Point ScreenPosition;

    public static void UpdatePositions()
    {
        MouseState m = Mouse.GetState();
        Point p = m.Position;

        int x = -(SpriteDrawer.RenderDestination.X - p.X);
        int y = -(SpriteDrawer.RenderDestination.Y - p.Y);

        x = x * GAME_WIDTH_IN_PIXELS / SpriteDrawer.RenderDestination.Width;
        y = y * GAME_HEIGHT_IN_PIXELS / SpriteDrawer.RenderDestination.Height;

        ScreenPosition = new Point(x, y);
    }
}