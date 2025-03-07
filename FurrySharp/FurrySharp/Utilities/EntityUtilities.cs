﻿using System;
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
    public static Rectangle BoundingBoxFromTexture(Texture2D texture)
    {
        return new Rectangle(0, 0, texture?.Width ?? 1, texture?.Height ?? 1);
    }
    
    public static Rectangle BoundingBoxFromSpritesheet(Spritesheet spritesheet)
    {
        return new Rectangle(0, 0, spritesheet?.Width ?? 1, spritesheet?.Height ?? 1);
    }

    public static bool SeparateEntityFromArea(Entity entity, FurRectangle area, Touching areaAllowCollisions, float overlapBias)
    {

        var b1 = SeparateEntityFromAreaX(entity, area, areaAllowCollisions, overlapBias);
        var b2 = SeparateEntityFromAreaY(entity, area, areaAllowCollisions, overlapBias);
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

    public static float GetEntityZ(Entity entity)
    {
        return DrawingUtilities.GetDrawingZ(DrawOrder.Entities, entity.Position.Y + entity.HitBox.Top, entity.Map.HeightInPixels);
    }
}