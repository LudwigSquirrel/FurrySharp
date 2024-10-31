using System;
using FurrySharp.Logging;
using FurrySharp.Resources;
using FurrySharp.Utilities;

namespace FurrySharp.Multiplatform;

public static class Program
{
    public static FurryGame FurryGame;

    [STAThread]
    static void Main()
    {
        RunNormally();
    }
    
    public static bool RunWithTest(GameLoopTestBundle test)
    {
        FurryGame = new FurryGame();
        FurryGame.Test = test;
        RunGame();
        return FurryGame.Test.Passed;
    }

    public static void RunNormally()
    {
        FurryGame = new FurryGame();
        RunGame();
    }

    private static void RunGame()
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