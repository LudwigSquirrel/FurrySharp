using System.Collections;
using FurrySharp.Input;
using FurrySharp.Logging;
using FurrySharp.Multiplatform;
using FurrySharp.States;
using FurrySharp.UI.Font;
using FurrySharp.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using static FurrySharp.Registry.GameConstants;

namespace FurrySharp.Test;

[TestFixture]
public class InGameTests
{
    private static SandboxState Sandbox => Program.FurryGame.CurrentState as SandboxState ?? throw new Exception("FurryGame didn't load the sandbox state cause it hates you >:3");
    [TestCaseSource(nameof(Cases))]
    public void InGameTest(IEnumerator test)
    {
        GameLoopTestBundle bundle = new GameLoopTestBundle()
        {
            TestRoutine = test
        };
        var result = Program.RunWithTest(bundle);
        Assert.IsTrue(result, "The test routine did not pass. Did somebody forget to 'yield return true' again :3");
    }

    private static IEnumerator TestRoutineTest(int frames, string message)
    {
        for (int i = 0; i < frames; i++)
        {
            DebugLogger.AddInfo($"Frame: {i}, {message}");
            yield return null;
        }

        yield return true;
    }

    private static IEnumerator TextWriterTest()
    {
        var textWriter = new UI.Font.Writing.TextWriter(new Point(0,0), 10, 10, FontManager.InitFont(true));
        textWriter.StupidWriteImmediate("Hello world!");
        yield return true;
    }

    private static IEnumerator TestPlayerWalkingUpTheTilesCheeks()
    {
        Sandbox.PlayerEntity.Position = new Vector2(TILE_SIZE * 1.5f, TILE_SIZE);
        GameInput.MockHeld(Keys.Up);
        yield return TimeSpan.FromSeconds(1);
        Assert.That(Sandbox.PlayerEntity.Position.Y, Is.GreaterThanOrEqualTo(0));
        yield return true;
    }

    public static object[] Cases =
    {
        TestRoutineTest(10, "This message will print 10 times!"),
        TextWriterTest(),
        TestPlayerWalkingUpTheTilesCheeks(),
    };
}