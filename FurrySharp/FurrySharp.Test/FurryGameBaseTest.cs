using FurrySharp.Multiplatform;

namespace FurrySharp.Test;

public class FurryGameBaseTest
{
    private FurryGame FurryGame => Program.FurryGame;
    
    [SetUp]
    public void SetupGame()
    {
        Thread gameThread = new Thread(() =>
        {
            Program.RunGame();
            // If the game exits, the test fails (or rather gets aborted but same same).
            Assert.Fail();
        });
        gameThread.Start();
        // Wait for the game to start.
        Thread.Sleep(1000);
    }
    
    [TearDown]
    public void TearDownGame()
    {
        FurryGame.Exit();
    }
}