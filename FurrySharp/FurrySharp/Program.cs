using System;
using FurrySharp.Logging;
using FurrySharp.Resources;

namespace FurrySharp.Multiplatform
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                ResourceManager.BaseDir = AppDomain.CurrentDomain.BaseDirectory;
                using var game = new FurryGame();
                game.Run();
            }
            catch (Exception ex)
            {
                DebugLogger.AddException(ex);
                throw;
            }
        }
    }
}