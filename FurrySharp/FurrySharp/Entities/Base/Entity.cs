using System;
using FurrySharp.Drawing;
using FurrySharp.Maps;
using FurrySharp.Registry;
using FurrySharp.Utilities;
using Microsoft.Xna.Framework;

namespace FurrySharp.Entities.Base;

[Flags]
public enum Touching
{
    NONE = 0,
    LEFT = 1,
    RIGHT = 2,
    UP = 4,
    DOWN = 8,
    ANY = LEFT | RIGHT | UP | DOWN
}

public enum Facing
{
    N,
    S,
    E,
    W,
}

public class Entity
{
    public int InstanceId;

    public EntityManager Manager; // the manager that bred me. Don't modify please :3
    public MapInfo Map => Manager.Map;

    public Vector2 Position;
    public Vector2 LastPosition;

    public Rectangle HitBox;
    public Rectangle BoundingBox;

    public float HitRadius;

    public Touching Touching = Touching.NONE;
    public Touching WasTouching = Touching.NONE;
    public Touching AllowCollisions = Touching.ANY;

    public Facing Facing = Facing.S;

    public bool Immovable;

    public virtual void Update()
    {
        // Do nothing
    }

    public virtual void PostUpdate()
    {
        LastPosition = Position;

        WasTouching = Touching;
        Touching = Touching.NONE;
    }

    public virtual void Draw()
    {
        if (GlobalState.DrawHitBoxes)
        {
            // Left, Right, Top, Bottom
            SpriteDrawer.DrawSprite(SpriteDrawer.SolidTex, new Rectangle((int)Position.X + HitBox.X, (int)Position.Y + HitBox.Y, 1, HitBox.Height), color: (WasTouching & Touching.LEFT) != 0 ? Color.Red : Color.White);
            SpriteDrawer.DrawSprite(SpriteDrawer.SolidTex, new Rectangle((int)Position.X + HitBox.X + HitBox.Width, (int)Position.Y + HitBox.Y, 1, HitBox.Height), color: (WasTouching & Touching.RIGHT) != 0 ? Color.Red : Color.White);
            SpriteDrawer.DrawSprite(SpriteDrawer.SolidTex, new Rectangle((int)Position.X + HitBox.X, (int)Position.Y + HitBox.Y, HitBox.Width, 1), color: (WasTouching & Touching.UP) != 0 ? Color.Red : Color.White);
            SpriteDrawer.DrawSprite(SpriteDrawer.SolidTex, new Rectangle((int)Position.X + HitBox.X, (int)Position.Y + HitBox.Y + HitBox.Height, HitBox.Width, 1), color: (WasTouching & Touching.DOWN) != 0 ? Color.Red : Color.White);
        }
        
        if (GlobalState.DrawHitRadii)
        {
            Vector2[] points = MathUtilities.PlotCircle(Position + HitBox.Center.ToVector2(), HitRadius, 8);
            for (var i = 0; i < points.Length; i++)
            {
                //SpriteDrawer.DrawSprite(SpriteDrawer.SolidTex, new Rectangle((int)points[i].X, (int)points[i].Y, 1, 1), color: Color.White);
                SpriteDrawer.DrawLine(points[i], points[(i + 1) % points.Length], Color.White);
            }
        }
    }

    public virtual void Collided(Entity other)
    {
        // Do nothing
    }
}