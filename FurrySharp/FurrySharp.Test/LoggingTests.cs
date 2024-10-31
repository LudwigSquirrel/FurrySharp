using FurrySharp.Logging;

namespace FurrySharp.Test;

public class LoggingTests
{
    [Test]
    public void DebugLoggerTest()
    {
        Console.Write("Hello World?"); // this prints in NUnit's output.
        DebugLogger.AddDebug("Test");
        DebugLogger.AddCritical("Test");
        DebugLogger.AddError("Test");
        DebugLogger.AddInfo("Test");
        DebugLogger.AddWarning("Test");
        Assert.Pass();
    }
}