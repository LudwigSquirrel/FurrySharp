﻿using System;
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

public class Entity
{
    public int InstanceId;

    public EntityManager Manager; // the manager that bred me. Don't modify please :3
    public MapInfo Map => Manager.Map;
    
    public Vector2 Position;
    public Vector2 LastPosition;

    public Rectangle HitBox;
    public Rectangle BoundingBox;

    public Touching Touching = Touching.NONE;
    public Touching WasTouching = Touching.NONE;
    public Touching AllowCollisions = Touching.ANY;

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
    }

    public virtual void Collided(Entity other)
    {
        // Do nothing
    }
}