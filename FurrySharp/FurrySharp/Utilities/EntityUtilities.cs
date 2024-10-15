using System;
using System.Drawing;
using FurrySharp.Entities.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace FurrySharp.Utilities;

public static class EntityUtilities
{
    public static Rectangle BoundingBoxFromTexture(Texture2D texture)
    {
        return new Rectangle(0, 0, texture.Width, texture.Height);
    }

    public static bool SeparateEntityFromArea(Entity entity, Rectangle area, Touching areaAllowCollisions, float overlapBias)
    {
        return SeparateEntityFromAreaX(entity, area, areaAllowCollisions, overlapBias) || SeparateEntityFromAreaY(entity, area, areaAllowCollisions, overlapBias);
    }

    private static bool SeparateEntityFromAreaX(Entity entity, Rectangle area, Touching areaAllowCollisions, float overlapBias)
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
    
    // Same thing for Y axis
    private static bool SeparateEntityFromAreaY(Entity entity, Rectangle area, Touching areaAllowCollisions, float overlapBias)
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
}