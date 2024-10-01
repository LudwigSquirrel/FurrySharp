using FurrySharp.Multiplatform;

namespace FurrySharp.Test.IntegrationTests;

[TestFixture]
public partial class IntegrationTests
{
    protected FurryGame FurryGame => Program.FurryGame;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        var gameThread = new Thread(() =>
        {
            Program.RunGame();
            // If the game exits, the test fails (or rather gets aborted but same same).
            Assert.Fail();
        });
        gameThread.Start();
        Thread.Sleep(1000);
    }

    [OneTimeTearDown]
    public void Teardown()
    {
        FurryGame.Exit();
    }
}