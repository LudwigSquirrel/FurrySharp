﻿using System;
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

#if DEBUG
    public static bool RunWithTest(GameLoopTestBundle test)
    {
        FurryGame = new FurryGame();
        FurryGame.Test = test;
        RunGame();
        return FurryGame.Test.Passed;
    }
#endif

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
            /*
             * Proof of concept: somehow run auto savers in event of crash.
             */
            // foreach (var autoSaver in FurryGame.AutoSavers)
            // {
            //     try
            //     {
            //         autoSaver.PerformSave();
            //     }
            //     catch
            //     {
            //         DebugLogger.AddCritical($"Could not perform save for resource {autoSaver.ResourceName}");
            //         DebugLogger.AddException(ex);
            //     }
            // }

            throw;
        }
    }
}