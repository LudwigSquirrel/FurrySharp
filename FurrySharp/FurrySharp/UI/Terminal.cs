using FurrySharp.Drawing;
using FurrySharp.Input;
using FurrySharp.Parsing;
using FurrySharp.Resources;
using FurrySharp.UI.Font;
using FurrySharp.UI.Font.Writing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using static FurrySharp.Registry.GameConstants;

namespace FurrySharp.UI;

public class Terminal
{
    private string InputText;
    private TextWriter InputWriter;
    private TextWriter OutputWriter;
    private Texture2D TerminalBG;

    public Terminal()
    {
        InputWriter = new TextWriter(new Point(1, 312), 68, 1, FontManager.InitFont(true));
        OutputWriter = new TextWriter(new Point(1, 8), 68, 38, FontManager.InitFont(true));
        TerminalBG = ResourceManager.GetTexture("terminal_bg");
    }

    public void Draw()
    {
        SpriteDrawer.DrawSprite(
            TerminalBG,
            sRect: new Rectangle((int)(GameTimes.TotalTime * 8), (int)(GameTimes.TotalTime * 4),GAME_WIDTH_IN_PIXELS, GAME_HEIGHT_IN_PIXELS),
            rect: new Rectangle(0, 0, GAME_WIDTH_IN_PIXELS, GAME_HEIGHT_IN_PIXELS),
            color: Color.White * 0.75f,
            z: DrawingUtilities.GetDrawingZ(DrawOrder.TerminalBG));
        InputWriter.Draw();
        OutputWriter.Draw();
    }

    public void TextInputHandler(object sender, TextInputEventArgs args)
    {
        var character = args.Character;
        
        if (args.Key == Keys.Back && InputText.Length > 0)
        {
            InputText = InputText.Remove(InputText.Length - 1);
        }
        else if (args.Key == Keys.Enter)
        {
            OutputWriter.StupidWriteImmediate($">>{InputText}\n");
            TerminalCommandParser.DoCommand(InputText, this);
            InputText = "";
        }
        else if (char.IsLetterOrDigit(character) || char.IsPunctuation(character) || char.IsWhiteSpace(character) || char.IsSymbol(character))
        {
            InputText += character;
        }
        
        
        InputWriter.ClearTextAndResetCursor();
        InputWriter.StupidWriteImmediate($">>{InputText}_");
    }
    
    public void PrintError(string error)
    {
        OutputWriter.WriterColor = Color.Red;
        OutputWriter.StupidWriteImmediate($"err: {error}\n");
        OutputWriter.WriterColor = Color.White;
    }
    
    public void PrintInfo(string info)
    {
        OutputWriter.WriterColor = Color.CornflowerBlue;
        OutputWriter.StupidWriteImmediate($"i: {info}\n");
        OutputWriter.WriterColor = Color.White;
    }

    public void ClearOutput()
    {
        OutputWriter.ClearTextAndResetCursor();
    }
}