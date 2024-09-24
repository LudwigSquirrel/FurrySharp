using FurrySharp.UI.Font;
using FurrySharp.UI.Font.Writing;
using Microsoft.Xna.Framework;

namespace FurrySharp.UI;

public class UILabel
{
    private string text;
    public TextWriter TextWriter { get; private set; }

    public string Text
    {
        get => text;
        set
        {
            text = value;
            TextWriter.ClearTextAndResetCursor();
            TextWriter.StupidWriteImmediate(text);
        }
    }

    public UILabel(Point position, int width, int height, SpriteFont font)
    {
        TextWriter = new TextWriter(position, width, height, font);
    }

    public void Draw()
    {
        TextWriter.Draw();
    }
}