using System.Collections.Generic;
using FurrySharp.Multiplatform;
using FurrySharp.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FurrySharp.Registry;
using FurrySharp.States;
using Microsoft.Xna.Framework.Content;
using static FurrySharp.Registry.GameConstants;

namespace FurrySharp.Drawing;

public static class SpriteDrawer
{
    private static GraphicsDevice graphicsDevice;

    private static Rectangle renderDestination;
    private static RenderTarget2D renderTargetNative; // for drawing to the screen, independent of the window size

    public static Color BackColor;
    public static Texture2D SolidTex;
    public static SpriteBatch SpriteBatch;
    public static Camera Camera = new Camera();

    public static void Initialize(GraphicsDevice device)
    {
        BackColor = Color.Black;

        graphicsDevice = device;
        SpriteBatch = new(graphicsDevice);

        renderTargetNative = new(graphicsDevice, GAME_WIDTH_IN_PIXELS, GAME_HEIGHT_IN_PIXELS);

        SolidTex = new(graphicsDevice, 2, 2);
        SolidTex.SetData(new Color[] { Color.White, Color.White, Color.White, Color.White });
    }

    public static void BeginDraw()
    {
        graphicsDevice.Clear(BackColor);

        graphicsDevice.SetRenderTarget(renderTargetNative);

        SpriteBatch.Begin(
            sortMode: SpriteSortMode.BackToFront,
            blendState: BlendState.AlphaBlend,
            samplerState: SamplerState.PointWrap, // best for pixel art
            effect: null,
            transformMatrix: Camera.Transform);
    }

    public static void EndDraw()
    {
        SpriteBatch.End();

        graphicsDevice.SetRenderTarget(null);

        SpriteBatch.Begin(
            sortMode: SpriteSortMode.BackToFront,
            blendState: BlendState.AlphaBlend,
            samplerState: SamplerState.PointWrap, // best for pixel art
            effect: null);
        SpriteBatch.Draw(renderTargetNative, renderDestination, Color.White);
        SpriteBatch.End();
    }

    public static void DrawSprite(Texture2D texture, Rectangle rect, Rectangle? sRect = null, Color? color = null, float rotation = 0, SpriteEffects flip = SpriteEffects.None, float z = 0)
    {
        var r = new Rectangle(rect.X + rect.Width / 2, rect.Y + rect.Height / 2, rect.Width, rect.Height);

        SpriteBatch.Draw(
            texture: texture,
            destinationRectangle: r,
            sourceRectangle: sRect,
            color: color ?? Color.White,
            rotation: rotation,
            // ReSharper disable twice PossibleLossOfFraction
            origin: new Vector2((sRect ?? texture.Bounds).Width / 2, (sRect ?? texture.Bounds).Height / 2),
            effects: flip,
            layerDepth: z);
    }

    public static void DrawSprite(Texture2D texture, Vector2 pos, Rectangle? sRect = null, Color? color = null, float rotation = 0, float scale = 1f, float z = 0)
    {
        SpriteBatch.Draw(
            texture: texture,
            position: pos,
            sourceRectangle: sRect,
            color: color ?? Color.White,
            rotation: rotation,
            origin: new Vector2(0),
            scale: scale,
            effects: SpriteEffects.None,
            layerDepth: z);
    }

    public static void UpdateRenderDestination(int displayWidth, int displayHeight, int scale)
    {
        renderDestination.Width = GAME_WIDTH_IN_PIXELS * scale;
        renderDestination.Height = GAME_HEIGHT_IN_PIXELS * scale;

        renderDestination.X = (displayWidth - renderDestination.Width) / 2;
        renderDestination.Y = (displayHeight - renderDestination.Height) / 2;
    }
}