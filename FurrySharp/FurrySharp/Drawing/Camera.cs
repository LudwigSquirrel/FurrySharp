using FurrySharp.Utilities;
using Microsoft.Xna.Framework;

using static FurrySharp.Registry.GameConstants;

namespace FurrySharp.Drawing;

public class Camera
{
    private Vector3 actualPos;
    
    public Matrix View { get; private set; }
    public Matrix Transform { get; private set; }
    
    public Vector3 Position
    {
        get
        {
            return new Vector3((int)actualPos.X, (int)actualPos.Y, actualPos.Z);
        }
    }
    public Vector3 Offset { get; set; }
    
    public Vector2 Position2D => new Vector2(Position.X, Position.Y) - new Vector2(GAME_WIDTH_IN_PIXELS, GAME_HEIGHT_IN_PIXELS) / 2;

    public Rectangle Bounds
    {
        get
        {
            Vector2 pos = Position2D;
            return new Rectangle((int)pos.X, (int)pos.Y, GAME_WIDTH_IN_PIXELS, GAME_HEIGHT_IN_PIXELS);
        }
    }

    public Camera()
    {
        actualPos = Vector3.Zero;
    }

    public void Recalc()
    {
        View = Matrix.CreateLookAt(Position, new Vector3(Position.X, Position.Y, -1), Vector3.Up);

        Transform = Matrix.CreateTranslation(-Position) *
                    Matrix.CreateTranslation(new Vector3(GAME_WIDTH_IN_PIXELS * 0.5f, GAME_HEIGHT_IN_PIXELS * 0.5f, 0)) *
                    Matrix.CreateTranslation(Offset);
    }
    
    public bool GoTowards(Vector2 target, float speed)
    {
        return MathUtilities.MoveTo(ref actualPos.X, target.X + GAME_WIDTH_IN_PIXELS / 2, speed) && MathUtilities.MoveTo(ref actualPos.Y, target.Y + GAME_HEIGHT_IN_PIXELS / 2, speed);
    }

    public void GoTo(Vector2 target)
    {
        target += new Vector2(GAME_WIDTH_IN_PIXELS, GAME_HEIGHT_IN_PIXELS) / 2;
        actualPos = new Vector3(target, actualPos.Z);
        Recalc();
    }

    public void CenterOn(Vector2 target)
    {
        actualPos = new Vector3(target, actualPos.Z);
        Recalc();
    }
    
    public Vector2 ScreenToWorld(Vector2 screenPos)
    {
        return Vector2.Transform(screenPos, Matrix.Invert(Transform));
    }
}