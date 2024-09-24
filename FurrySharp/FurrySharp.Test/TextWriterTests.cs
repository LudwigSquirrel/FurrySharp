using FurrySharp.Dialogue;
using FurrySharp.Multiplatform;
using FurrySharp.Registry;
using FurrySharp.States;
using FurrySharp.UI.Font;
using Microsoft.Xna.Framework;
using TextWriter = FurrySharp.UI.Font.Writing.TextWriter;

namespace FurrySharp.Test.WritingTests;

public class TextWriterTests : FurryGameBaseTest
{
    public TextWriter TextWriter;
    
    [Test]
    public void ConstructorTest()
    {
        TextWriter = new TextWriter(new Point(0,0), 10, 10, FontManager.InitFont(true));
        Assert.Pass();
    }
}