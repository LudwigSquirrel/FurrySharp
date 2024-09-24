using FurrySharp.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FurrySharp.UI.Font
{
    public class SpriteFont
    {
        private readonly int lineHeight;
        private readonly int spaceWidth;

        public Texture2D Texture { get; set; }

        public int LineSeparation => lineHeight + 2;

        private readonly Dictionary<char, Rectangle> characterRectangles;

        public SpriteFont(int lineHeight, int spaceWidth, string textureName, string characterOrder)
        {
            this.lineHeight = lineHeight;
            this.spaceWidth = spaceWidth;

            Texture = ResourceManager.GetTexture(textureName, true);
            characterRectangles = CreateRectangles(characterOrder);
        }

        public Rectangle? GetRectangle(char character)
        {
            if (characterRectangles.TryGetValue(character, out var rectangle))
            {
                return rectangle;
            }

            return null;
        }

        private Dictionary<char, Rectangle> CreateRectangles(string characterOrder)
        {
            var d = new Dictionary<char, Rectangle>();
            var texWidth = Texture.Bounds.Width;
            var charsPerLine = texWidth / spaceWidth;

            for (var i = 0; i < characterOrder.Length; i++)
            {
                var c = characterOrder[i];
                if (c == ' ' || d.ContainsKey(c))
                {
                    continue;
                }

                var indexX = i % charsPerLine * spaceWidth;
                var indexY = i / charsPerLine * lineHeight;

                d.Add(characterOrder[i], new Rectangle(indexX, indexY, spaceWidth, lineHeight));
            }

            return d;
        }
    }
}