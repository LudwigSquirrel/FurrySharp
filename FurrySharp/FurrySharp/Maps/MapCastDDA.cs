using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FurrySharp.Entities.Base;
using FurrySharp.Maps.Tiles;
using Microsoft.Xna.Framework;
using static FurrySharp.Registry.GameConstants;

namespace FurrySharp.Maps;

public struct Result
{
    public bool TileFound;
    public Point TilePosition;
    public Vector2 End;
    public List<Point> Visited;
}

public static class MapCastDDA
{
    public static Result DDA(this MapInfo map, Vector2 vStart, Vector2 vEnd)
    {
        Vector2 normalized = Vector2.Normalize(vEnd - vStart);
        return DDA(map, vStart, normalized, Vector2.Distance(vStart, vEnd));
    }

    // Differential Digital Analysis (https://www.youtube.com/watch?v=NbSee-XM7WA&t=586s)
    // Direction is assumed to be normalized.
    public static Result DDA(this MapInfo map, Vector2 vStart, Vector2 vDirection, float maxDistance)
    {
        // vStart is the starting position of the ray. It is divided by TILE_SIZE because those are our units.
        vStart /= TILE_SIZE;
        // vRayUnitStepSize represents the distance the ray needs to travel along the x and y axes
        // to move from one grid line to the next. It is calculated based on the direction (a unit vector) of the ray
        // and ensures that the ray progresses uniformly in both x and y directions.
        Vector2 vRayUnitStepSize = new Vector2(
            x: MathF.Sqrt(1 + ((vDirection.Y / vDirection.X) * (vDirection.Y / vDirection.X))),
            y: MathF.Sqrt(1 + ((vDirection.X / vDirection.Y) * (vDirection.X / vDirection.Y))));
        // vMapCheck is the tile we will be looking up from the map.
        // Collision data will be read from the tile, and put through a bit mask test; determining if the ray collides
        // or passes through.
        Point vMapCheck = vStart.ToPoint();
        // vRayLength1D.X stores the length of the ray in accumulated columns.
        // vRayLength1D.Y stores the length of the ray in accumulated rows.
        // We will need this so we can compare which is shorter, and that information will tell us the next "step" we
        // take in the map.
        Vector2 vRayLength1D;
        // vStep stores the values we use to increment/decrement vMapCheck.
        Point vStep;
        // collisionMask stores what bit flags will stop the ray. This is based on the ray's direction.
        Touching collisionMask = Touching.NONE;
        
        if (vDirection.X < 0)
        {
            vStep.X = -1;
            // We need to manually calculate the length of the ray to the first intersection point.
            vRayLength1D.X = (vStart.X - vMapCheck.X) * vRayUnitStepSize.X;
            collisionMask |= Touching.RIGHT;
        }
        else
        {
            vStep.X = 1;
            vRayLength1D.X = (vMapCheck.X + 1 - vStart.X) * vRayUnitStepSize.X;
            collisionMask |= Touching.LEFT;
        }
        
        if (vDirection.Y < 0)
        {
            vStep.Y = -1;
            vRayLength1D.Y = (vStart.Y - vMapCheck.Y) * vRayUnitStepSize.Y;
            collisionMask |= Touching.DOWN;
        }
        else
        {
            vStep.Y = 1;
            vRayLength1D.Y = (vMapCheck.Y + 1 - vStart.Y) * vRayUnitStepSize.Y;
            collisionMask |= Touching.UP;
        }

        bool bTileFound = false;
        float fDistance = 0f;
        List<Point> visited = new List<Point>();
        while (!bTileFound && fDistance < maxDistance)
        {
            visited.Add(vMapCheck);
            // Walk
            if (vRayLength1D.X < vRayLength1D.Y)
            {
                vMapCheck.X += vStep.X;
                fDistance = vRayLength1D.X;
                vRayLength1D.X += vRayUnitStepSize.X;
            }
            else
            {
                vMapCheck.Y += vStep.Y;
                fDistance = vRayLength1D.Y;
                vRayLength1D.Y += vRayUnitStepSize.Y;
            }
            
            // Check for collision.
            int tileIndex = map.GetTile((int)MapLayer.BG, vMapCheck.X, vMapCheck.Y);
            TileDataContainer.TileData data = map.TileDataContainer.GetData(tileIndex);
            if ((data.CollisionDirection & collisionMask) != 0)
            {
                bTileFound = true;
            }
        }
        return new Result()
        {
            TileFound = bTileFound,
            TilePosition = vMapCheck,
            End = (vStart * TILE_SIZE) + (vDirection * (fDistance * TILE_SIZE)),
            Visited = visited,
        };
    }
}