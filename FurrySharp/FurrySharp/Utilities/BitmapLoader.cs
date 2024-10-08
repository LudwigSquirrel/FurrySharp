using System;
using System.IO;

namespace FurrySharp.Utilities;

public static class BitmapLoader
{
    public static int[,] LoadPixelData(string filePath)
    {
        {
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var reader = new BinaryReader(stream);

            // Read the bitmap file header
            reader.BaseStream.Seek(0x12, SeekOrigin.Begin);
            int widthInPixels = reader.ReadInt32();
            int heightInPixels = reader.ReadInt32();

            // Read the pixel data offset
            reader.BaseStream.Seek(0xA, SeekOrigin.Begin);
            int pixelDataOffset = reader.ReadInt32();

            // Read the bits per pixel
            reader.BaseStream.Seek(0x1C, SeekOrigin.Begin);
            int bitsPerPixel = reader.ReadInt16();
            int bytesPerScanline = (int)Math.Ceiling((bitsPerPixel * widthInPixels) / 32.0) * 4;

            // Read the pixel data
            reader.BaseStream.Seek(pixelDataOffset, SeekOrigin.Begin);
            int[,] indices = new int[widthInPixels,heightInPixels];
            switch (bitsPerPixel)
            {
                case 4:
                    IterateAt4BitsPerPixel();
                    break;
                case 8:
                    IterateAt8BitsPerPixel();
                    break;
                default:
                    throw new NotSupportedException("Only 4 and 8 bits per pixel are supported.");
            }

            return indices;

            void IterateAt4BitsPerPixel()
            {
                for (int j = heightInPixels - 1; j >= 0; j--)
                {
                    byte[] bytes = reader.ReadBytes(bytesPerScanline);
                    for (int i = 0; i < widthInPixels; i++)
                    {
                        var getFrontHalf = i % 2 == 0;
                        var byteIndex = i / 2;
                        var byteValue = bytes[byteIndex];
                        var shift = getFrontHalf ? 4 : 0;
                        var mask = 0b1111 << shift;
                        var index = (byteValue & mask) >> shift;
                        indices[i,j] = index;
                    }
                }
            }

            void IterateAt8BitsPerPixel()
            {
                for (int j = 0; j < heightInPixels; j++)
                {
                    byte[] bytes = reader.ReadBytes(bytesPerScanline);
                    for (int i = 0; i < widthInPixels; i++)
                    {
                        indices[i,j] = bytes[i];
                    }
                }
            }
        }
    }
}