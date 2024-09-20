using FurrySharp.Logging;

namespace FurrySharp.Test;

public class LoggingTests
{
    [Test]
    public void DebugLoggerTest()
    {
        DebugLogger.AddDebug("Test");
        DebugLogger.AddCritical("Test");
        DebugLogger.AddError("Test");
        DebugLogger.AddInfo("Test");
        DebugLogger.AddWarning("Test");
        Assert.Pass();
    }
}