using Microsoft.Xna.Framework;

namespace FurrySharp.UI.Font.Writing;

public class TextCharacter
{
    public static TextCharacter NewEmpty => new(null, null, Color.White);
    
    public Color Color { get; set; }
    public Rectangle? Crop { get; set; }
    public char? Character { get; set; }

    private TextCharacter(char? character, Rectangle? crop, Color color)
    {
        Character = character;
        Crop = crop;
        Color = color;
    }
}