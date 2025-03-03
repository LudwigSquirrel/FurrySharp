using System;
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

    private static RenderTarget2D gameTarget;
    private static RenderTarget2D uiTarget;
    
    private static RenderTarget2D renderTargetNative; // for drawing to the screen, independent of the window size

    public static Rectangle RenderDestination;
    public static Color BackColor;
    public static Texture2D SolidTex;
    public static SpriteBatch SpriteBatch;
    public static Camera Camera = new Camera();

    public static void Initialize(GraphicsDevice device)
    {
        BackColor = Color.Black;

        graphicsDevice = device;
        SpriteBatch = new(graphicsDevice);

        gameTarget = new(graphicsDevice, GAME_WIDTH_IN_PIXELS, GAME_HEIGHT_IN_PIXELS);
        uiTarget = new(graphicsDevice, GAME_WIDTH_IN_PIXELS, GAME_HEIGHT_IN_PIXELS);
        renderTargetNative = new(graphicsDevice, GAME_WIDTH_IN_PIXELS, GAME_HEIGHT_IN_PIXELS);

        SolidTex = new(graphicsDevice, 2, 2);
        SolidTex.SetData(new Color[] { Color.White, Color.White, Color.White, Color.White });
    }

    public static void BeginDraw()
    {
        graphicsDevice.Clear(BackColor);

        graphicsDevice.SetRenderTarget(gameTarget);

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
    }

    public static void BeginGUIDraw()
    {
        graphicsDevice.SetRenderTarget(uiTarget);
        graphicsDevice.Clear(BackColor);
        
        SpriteBatch.Begin();
        SpriteBatch.Draw(gameTarget, new Rectangle(0,0, GAME_WIDTH_IN_PIXELS, GAME_HEIGHT_IN_PIXELS), Color.White);
        SpriteBatch.End();
        
        SpriteBatch.Begin(
            sortMode: SpriteSortMode.BackToFront,
            blendState: BlendState.AlphaBlend,
            samplerState: SamplerState.PointWrap, // best for pixel art
            effect: null);
    }

    public static void EndGUIDraw()
    {
        SpriteBatch.End();
    }

    public static void Render()
    {
        graphicsDevice.SetRenderTarget(null);
        
        SpriteBatch.Begin(
            sortMode: SpriteSortMode.Texture,
            blendState: BlendState.AlphaBlend,
            samplerState: SamplerState.PointWrap);
        SpriteBatch.Draw(uiTarget, RenderDestination, Color.White);
        SpriteBatch.End();
    }

    public static void DrawSprite(Texture2D texture, Rectangle rect, Rectangle? sRect = null, Color? color = null, float rotation = 0, Vector2? origin = null, SpriteEffects flip = SpriteEffects.None, float z = 0)
    {
        var r = new Rectangle(rect.X + rect.Width / 2, rect.Y + rect.Height / 2, rect.Width, rect.Height);

        SpriteBatch.Draw(
            texture: texture,
            destinationRectangle: r,
            sourceRectangle: sRect,
            color: color ?? Color.White,
            rotation: rotation,
            // ReSharper disable twice PossibleLossOfFraction
            origin: origin ?? new Vector2((sRect ?? texture.Bounds).Width / 2, (sRect ?? texture.Bounds).Height / 2),
            effects: flip,
            layerDepth: z);
    }

    // todo: rethink drawing commands. scale is not intuitive.
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

    public static void DrawLine(Vector2 start, Vector2 end, Color color, float z = 0)
    {
        var edge = end - start;
        var angle = (float)Math.Atan2(edge.Y, edge.X);

        SpriteBatch.Draw(
            SolidTex,
            new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), 1),
            null,
            color,
            angle,
            new Vector2(0, 0),
            SpriteEffects.None,
            z);
    }

    public static void DrawLines(Vector2[] points, Color color, bool closed = true, float z = 0)
    {
        var length = closed ? points.Length : points.Length - 1;
        for (var i = 0; i < length; i++)
        {
            DrawLine(points[i], points[(i + 1) % points.Length], color, z);
        }
    }

    public static void DrawDebugLine(Vector2 start, Vector2 end, Color color)
    {
        DrawLine(start, end, color, DrawingUtilities.GetDrawingZ(DrawOrder.DebugLine));
    }

    public static void UpdateRenderDestination(int displayWidth, int displayHeight, int scale)
    {
        RenderDestination.Width = GAME_WIDTH_IN_PIXELS * scale;
        RenderDestination.Height = GAME_HEIGHT_IN_PIXELS * scale;

        RenderDestination.X = (displayWidth - RenderDestination.Width) / 2;
        RenderDestination.Y = (displayHeight - RenderDestination.Height) / 2;
    }
}