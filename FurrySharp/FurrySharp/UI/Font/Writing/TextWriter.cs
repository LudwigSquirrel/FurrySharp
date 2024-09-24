using System.Collections.Generic;
using FurrySharp.Drawing;
using FurrySharp.Registry;
using Microsoft.Xna.Framework;

namespace FurrySharp.UI.Font.Writing;

public class TextWriter
{
    public Point Position;

    public readonly int Width; // in characters

    public readonly int Height; // in characters

    public SpriteFont Font;

    private TextCharacter[,] textCharacters; // The actual characters to draw and their positions.

    private Point WriterCursor; // The current position of the writer cursor.

    public Color WriterColor { get; set; } = Color.White; // The color text will be written in.

    public TextWriter(Point position, int width, int height, SpriteFont font)
    {
        Position = position;
        Width = width;
        Height = height;
        Font = font;

        InitTextCharacters();
        ClearTextAndResetCursor();
    }

    private void InitTextCharacters()
    {
        textCharacters = new TextCharacter[Width, Height];
        for (var i = 0; i < textCharacters.Length; i++)
        {
            var x = i % Width;
            var y = i / Width;
            textCharacters[x, y] = TextCharacter.NewEmpty;
        }
    }

    public void Draw()
    {
        var drawingZ = DrawingUtilites.GetDrawingZ(DrawOrder.Text);
        var characterWidth = FontManager.GetCharacterWidth();
        var fontLineHeight = GameConstants.FontLineHeight;
        var dRect = new Rectangle(0, 0, characterWidth, fontLineHeight); // The destination rectangle for the text characters. 

        if (GlobalState.DrawTextWriterAreas)
        {
            var rect = new Rectangle(Position, new Point(Width * characterWidth, Height * fontLineHeight));
            SpriteDrawer.DrawSprite(SpriteDrawer.SolidTex, rect, color: GameConstants.DebugTextAreaColor, z: drawingZ + 0.01f);
        }

        // Iterate through each text character, and draw it.
        for (var i = 0; i < textCharacters.Length; i++)
        {
            var x = i % Width;
            var y = i / Width;
            var tc = textCharacters[x, y];
            if (tc.Character is null or ' ')
            {
                continue;
            }

            dRect.X = Position.X + x * characterWidth;
            dRect.Y = Position.Y + y * fontLineHeight;
            SpriteDrawer.DrawSprite(Font.Texture, dRect, tc.Crop, tc.Color);
        }
    }

    public void StupidWriteImmediate(string text)
    {
        try
        {
            // Iterate through each character in the text.
            foreach (var c in text)
            {
                // If the character is a line break, move the writer cursor to the next line.
                if (c == '\n')
                {
                    WriterCursor.X = 0;
                    WriterCursor.Y++;
                }
                else
                {
                    Rectangle? crop = Font.GetRectangle(c);

                    // Write the character to the text writer.
                    TextCharacter tc = textCharacters[WriterCursor.X, WriterCursor.Y];
                    tc.Color = WriterColor;
                    tc.Crop = crop;
                    tc.Character = c;
                    
                    // Move the writer cursor to the next position.
                    WriterCursor.X++;
                    if (WriterCursor.X >= Width)
                    {
                        WriterCursor.X = 0;
                        WriterCursor.Y++;
                    }
                }
            }
        }
        catch
        {
            // Do nothing. It's your fault for using a method called stupid write.
        }
    }

    public void ClearTextAndResetCursor()
    {
        WriterCursor = new Point(0, 0);
        for (var i = 0; i < textCharacters.Length; i++)
        {
            var x = i % Width;
            var y = i / Width;
            textCharacters[x, y].Character = null;
        }
    }
}