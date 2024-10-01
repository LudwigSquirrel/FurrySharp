using FurrySharp.Dialogue;
using FurrySharp.Multiplatform;
using FurrySharp.Registry;
using FurrySharp.States;
using FurrySharp.UI.Font;
using Microsoft.Xna.Framework;
using TextWriter = FurrySharp.UI.Font.Writing.TextWriter;

namespace FurrySharp.Test.IntegrationTests;

public partial class IntegrationTests
{
    [Test]
    public void TextWriterConstructorTest()
    {
        var textWriter = new TextWriter(new Point(0,0), 10, 10, FontManager.InitFont(true));
        Assert.Pass();
    }
    
    [Test]
    public void StupidWriteImmediateTest()
    {
        var textWriter = new TextWriter(new Point(0,0), 10, 10, FontManager.InitFont(true));
        textWriter.StupidWriteImmediate("Hello world!");
        Assert.Pass();
    }
}