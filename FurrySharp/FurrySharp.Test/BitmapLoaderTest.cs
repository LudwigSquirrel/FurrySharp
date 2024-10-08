using FurrySharp.Utilities;

namespace FurrySharp.Test;

public class BitmapLoaderTest
{
    [Test]
    public void LoadTest()
    {
        var data = BitmapLoader.LoadPixelData("test.bmp"); // 3x3 bitmap with 4 bits per pixel
        // Iterate over the data. Print data in a grid.
        for (int y = 0; y < data.GetLength(1); y++)
        {
            for (int x = 0; x < data.GetLength(0); x++)
            {
                Console.Write(data[x, y]);
            }

            Console.WriteLine();
        }

        /*
         * Expected output:
         * 012
         * 120
         * 201
         */
        Assert.That(data[0, 0], Is.EqualTo(0));
        Assert.That(data[1, 0], Is.EqualTo(1));
        Assert.That(data[2, 0], Is.EqualTo(2));
        Assert.That(data[0, 1], Is.EqualTo(1));
        Assert.That(data[1, 1], Is.EqualTo(2));
        Assert.That(data[2, 1], Is.EqualTo(0));
        Assert.That(data[0, 2], Is.EqualTo(2));
        Assert.That(data[1, 2], Is.EqualTo(0));
        Assert.That(data[2, 2], Is.EqualTo(1));
    }
}