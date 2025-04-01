using System;
using System.Drawing;
using FurrySharp.Drawing;
using FurrySharp.Entities.Base;
using FurrySharp.Maps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace FurrySharp.Utilities;

public static class EntityUtilities
{
    public static Rectangle BoundingBoxFrom(Texture2D texture)
    {
        return new Rectangle(0, 0, texture?.Width ?? 1, texture?.Height ?? 1);
    }

    public static Rectangle BoundingBoxFrom(Spritesheet spritesheet)
    {
        return new Rectangle(0, 0, spritesheet?.Width ?? 1, spritesheet?.Height ?? 1);
    }

    public static float GetEntityZ(Entity entity)
    {
        return DrawingUtilities.GetDrawingZ(DrawOrder.Entities, entity.Position.Y + entity.HitBox.Top, entity.Map.HeightInPixels);
    }

    public static bool SeparateEntityFromArea(Entity entity, FurRectangle area, Touching areaAllowCollisions, float overlapBias)
    {
        var b1 = SeparateEntityFromAreaX(entity, area, areaAllowCollisions, overlapBias);
        var b2 = SeparateEntityFromAreaY(entity, area, areaAllowCollisions, overlapBias);
        return b1 || b2;
    }

    public static bool SeparateEntityFromEntity(Entity entity, Entity other, float overlapBias = 4f)
    {
        var b1 = SeparateEntityFromEntityX(entity, other, overlapBias);
        var b2 = SeparateEntityFromEntityY(entity, other, overlapBias);
        return b1 || b2;
    }

    private static bool SeparateEntityFromAreaX(Entity entity, FurRectangle area, Touching areaAllowCollisions, float overlapBias)
    {
        if (entity.Immovable)
        {
            return false;
        }

        // Get delta position. This will be used to grow the hit box.
        var delta = entity.Position.X - entity.LastPosition.X;
        var deltaAbs = MathF.Abs(delta);

        FurRectangle deltaBox = new(
            location: new Vector2(entity.LastPosition.X + entity.HitBox.X + (delta < 0 ? delta : 0), entity.LastPosition.Y + entity.HitBox.Y),
            size: new Vector2(entity.HitBox.Width + deltaAbs, entity.HitBox.Height)
        );

        if (deltaBox.Intersects(area) == false)
        {
            return false;
        }

        float overlap;
        float maxOverlap = deltaAbs + overlapBias;

        // If they did overlap (or the delta indicates they will), figure out by how much and flip the corresponding flags.
        float overlapRight = deltaBox.Right - area.Left;
        float overlapLeft = deltaBox.Left - area.Right;

        if (MathF.Abs(overlapRight) < MathF.Abs(overlapLeft))
        {
            overlap = overlapRight;
            if ((overlap > maxOverlap) || (entity.AllowCollisions & Touching.RIGHT) == 0 || (areaAllowCollisions & Touching.LEFT) == 0)
            {
                // This entity is allowed to pass through!
                overlap = 0;
            }
            else
            {
                entity.Touching |= Touching.RIGHT;
            }
        }
        else
        {
            overlap = overlapLeft;
            if ((-overlap > maxOverlap) || (entity.AllowCollisions & Touching.LEFT) == 0 || (areaAllowCollisions & Touching.RIGHT) == 0)
            {
                overlap = 0;
            }
            else
            {
                entity.Touching |= Touching.LEFT;
            }
        }

        if (overlap != 0)
        {
            entity.Position.X -= overlap;
            entity.LastPosition.X = entity.Position.X;
            return true;
        }

        return false;
    }

    private static bool SeparateEntityFromAreaY(Entity entity, FurRectangle area, Touching areaAllowCollisions, float overlapBias)
    {
        if (entity.Immovable)
        {
            return false;
        }

        var delta = entity.Position.Y - entity.LastPosition.Y;
        var deltaAbs = MathF.Abs(delta);

        FurRectangle deltaBox = new(
            location: new Vector2(entity.LastPosition.X + entity.HitBox.X, entity.LastPosition.Y + entity.HitBox.Y + (delta < 0 ? delta : 0)),
            size: new Vector2(entity.HitBox.Width, entity.HitBox.Height + deltaAbs)
        );

        if (deltaBox.Intersects(area) == false)
        {
            return false;
        }

        float overlap;
        float maxOverlap = deltaAbs + overlapBias;

        float overlapBottom = deltaBox.Bottom - area.Top;
        float overlapTop = deltaBox.Top - area.Bottom;

        if (MathF.Abs(overlapBottom) < MathF.Abs(overlapTop))
        {
            overlap = overlapBottom;
            if ((overlap > maxOverlap) || (entity.AllowCollisions & Touching.DOWN) == 0 || (areaAllowCollisions & Touching.UP) == 0)
            {
                overlap = 0;
            }
            else
            {
                entity.Touching |= Touching.DOWN;
            }
        }
        else
        {
            overlap = overlapTop;
            if ((-overlap > maxOverlap) || (entity.AllowCollisions & Touching.UP) == 0 || (areaAllowCollisions & Touching.DOWN) == 0)
            {
                overlap = 0;
            }
            else
            {
                entity.Touching |= Touching.UP;
            }
        }

        if (overlap != 0)
        {
            entity.Position.Y -= overlap;
            entity.LastPosition.Y = entity.Position.Y;
            return true;
        }

        return false;
    }

    private static bool SeparateEntityFromEntityX(Entity object1, Entity object2, float overlapBias)
    {
        // Can't separate two immovable entities.
        if (object1.Immovable && object2.Immovable)
        {
            return false;
        }

        // Get delta position. This will be used to grow the hit box.
        var obj1delta = object1.Position.X - object1.LastPosition.X;
        var obj2delta = object2.Position.X - object2.LastPosition.X;
        var obj1DeltaAbs = MathF.Abs(obj1delta);
        var obj2DeltaAbs = MathF.Abs(obj2delta);

        FurRectangle obj1DeltaBox = new(
            location: new Vector2(object1.LastPosition.X + object1.HitBox.X + (obj1delta < 0 ? obj1delta : 0), object1.LastPosition.Y + object1.HitBox.Y),
            size: new Vector2(object1.HitBox.Width + obj1DeltaAbs, object1.HitBox.Height)
        );
        FurRectangle obj2DeltaBox = new(
            location: new Vector2(object2.LastPosition.X + object2.HitBox.X + (obj2delta < 0 ? obj2delta : 0), object2.LastPosition.Y + object2.HitBox.Y),
            size: new Vector2(object2.HitBox.Width + obj2DeltaAbs, object2.HitBox.Height)
        );

        if (obj1DeltaBox.Intersects(obj2DeltaBox) == false)
        {
            return false;
        }

        float overlap;
        float maxOverlap = obj1DeltaAbs + obj2DeltaAbs + overlapBias;

        // If they did overlap (or the delta indicates they will), figure out by how much and flip the corresponding flags.
        float overlapRight = obj1DeltaBox.Right - obj2DeltaBox.Left;
        float overlapLeft = obj1DeltaBox.Left - obj2DeltaBox.Right;

        if (MathF.Abs(overlapRight) < MathF.Abs(overlapLeft))
        {
            overlap = overlapRight;
            if ((overlap > maxOverlap) || (object1.AllowCollisions & Touching.RIGHT) == 0 || (object2.AllowCollisions & Touching.LEFT) == 0)
            {
                // This entity is allowed to pass through!
                overlap = 0;
            }
            else
            {
                object1.Touching |= Touching.RIGHT;
                object2.Touching |= Touching.LEFT;
            }
        }
        else
        {
            overlap = overlapLeft;
            if ((-overlap > maxOverlap) || (object1.AllowCollisions & Touching.LEFT) == 0 || (object2.AllowCollisions & Touching.RIGHT) == 0)
            {
                overlap = 0;
            }
            else
            {
                object1.Touching |= Touching.LEFT;
                object2.Touching |= Touching.RIGHT;
            }
        }

        if (overlap != 0)
        {
            if (!object1.Immovable && !object2.Immovable)
            {
                overlap *= 0.5f;
                object1.Position.X -= overlap;
                object1.LastPosition.X = object1.Position.X;
                object2.Position.X += overlap;
                object2.LastPosition.X = object2.Position.X;
                return true;
            }

            if (!object1.Immovable)
            {
                object1.Position.X -= overlap;
                object1.LastPosition.X = object1.Position.X;
                return true;
            }

            if (!object2.Immovable)
            {
                object2.Position.X += overlap;
                object2.LastPosition.X = object2.Position.X;
                return true;
            }
        }

        return false;
    }

    private static bool SeparateEntityFromEntityY(Entity object1, Entity object2, float overlapBias)
    {
        // Can't separate two immovable entities.
        if (object1.Immovable && object2.Immovable)
        {
            return false;
        }

        // Get delta position. This will be used to grow the hit box.
        var obj1delta = object1.Position.Y - object1.LastPosition.Y;
        var obj2delta = object2.Position.Y - object2.LastPosition.Y;
        var obj1DeltaAbs = MathF.Abs(obj1delta);
        var obj2DeltaAbs = MathF.Abs(obj2delta);

        FurRectangle obj1DeltaBox = new(
            location: new Vector2(object1.LastPosition.Y + object1.HitBox.X + (obj1delta < 0 ? obj1delta : 0), object1.LastPosition.Y + object1.HitBox.Y),
            size: new Vector2(object1.HitBox.Width + obj1DeltaAbs, object1.HitBox.Height)
        );
        FurRectangle obj2DeltaBox = new(
            location: new Vector2(object2.LastPosition.Y + object2.HitBox.X + (obj2delta < 0 ? obj2delta : 0), object2.LastPosition.Y + object2.HitBox.Y),
            size: new Vector2(object2.HitBox.Width + obj2DeltaAbs, object2.HitBox.Height)
        );

        if (obj1DeltaBox.Intersects(obj2DeltaBox) == false)
        {
            return false;
        }

        float overlap;
        float maxOverlap = obj1DeltaAbs + obj2DeltaAbs + overlapBias;

        // If they did overlap (or the delta indicates they will), figure out by how much and flip the corresponding flags.
        float overlapDown = obj1DeltaBox.Bottom - obj2DeltaBox.Top;
        float overlapUp = obj1DeltaBox.Top - obj2DeltaBox.Bottom;

        if (MathF.Abs(overlapDown) < MathF.Abs(overlapUp))
        {
            overlap = overlapDown;
            if ((overlap > maxOverlap) || (object1.AllowCollisions & Touching.DOWN) == 0 || (object2.AllowCollisions & Touching.UP) == 0)
            {
                // This entity is allowed to pass through!
                overlap = 0;
            }
            else
            {
                object1.Touching |= Touching.DOWN;
                object2.Touching |= Touching.UP;
            }
        }
        else
        {
            overlap = overlapUp;
            if ((-overlap > maxOverlap) || (object1.AllowCollisions & Touching.UP) == 0 || (object2.AllowCollisions & Touching.DOWN) == 0)
            {
                overlap = 0;
            }
            else
            {
                object1.Touching |= Touching.UP;
                object2.Touching |= Touching.DOWN;
            }
        }

        if (overlap != 0)
        {
            if (!object1.Immovable && !object2.Immovable)
            {
                overlap *= 0.5f;
                object1.Position.Y -= overlap;
                object1.LastPosition.Y = object1.Position.Y;
                object2.Position.Y += overlap;
                object2.LastPosition.Y = object2.Position.Y;
                return true;
            }

            if (!object1.Immovable)
            {
                object1.Position.Y -= overlap;
                object1.LastPosition.Y = object1.Position.Y;
                return true;
            }

            if (!object2.Immovable)
            {
                object2.Position.Y += overlap;
                object2.LastPosition.Y = object2.Position.Y;
                return true;
            }
        }

        return false;
    }
}