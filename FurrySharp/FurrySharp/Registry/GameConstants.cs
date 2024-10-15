using System;
using System.IO;
using FurrySharp.Dialogue;
using Microsoft.Xna.Framework;

namespace FurrySharp.Registry;

public class GameConstants
{
    public const int GAME_WIDTH_IN_PIXELS = 480;
    public const int GAME_HEIGHT_IN_PIXELS = 320;
    public const int TILE_SIZE = 32;
    public const int SUPPORTED_TILE_COUNT = 255;
    
    public static readonly string SavePath;
    public static Color DebugTextAreaColor = new(128, 0, 0, 128);

    static GameConstants()
    {
        SavePath =
#if DEBUG
        "./";
#else
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData,Environment.SpecialFolderOption.Create),"FurrySharp/");
#endif
    }

    public static int FontLineHeight => 8;

    public static int LineOffset => FontLineHeight - 8;
}