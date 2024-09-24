using System;
using System.Threading;
using FurrySharp.Logging;
using FurrySharp.Resources;
using Microsoft.Xna.Framework;

namespace FurrySharp.Multiplatform;

public static class Program
{
    public static readonly FurryGame FurryGame = new();

    [STAThread]
    static void Main()
    {
        RunGame();
    }

    public static void RunGame()
    {
        try
        {
            ResourceManager.BaseDir = AppDomain.CurrentDomain.BaseDirectory;
            FurryGame.Run();
        }
        catch (Exception ex)
        {
            DebugLogger.AddException(ex);
            throw;
        }
    }
}