using FurrySharp.Drawing;
using FurrySharp.Input;
using FurrySharp.Resources;
using Microsoft.Xna.Framework;

namespace FurrySharp.UI;

public class FingyCursor
{
    public Spritesheet FingySheet;

    public FingyCursor()
    {
        FingySheet = new Spritesheet(ResourceManager.GetTexture("pointer"), 16, 16);
    }

    public void DrawFingy()
    {
        Rectangle source = FingySheet.GetRect(0);
        var screenPosition = MouseInput.ScreenPosition;
        Rectangle dest = new Rectangle(screenPosition, new Point(16));
        var drawingZ = DrawingUtilities.GetDrawingZ(DrawOrder.Fingy);
        SpriteDrawer.DrawSprite(FingySheet.Tex, dest, source, z: drawingZ);
    }
}