using System;
using FurrySharp.Entities.Base;
using FurrySharp.Utilities;
using Microsoft.Xna.Framework;

namespace FurrySharp.Entities.Components;

public class PathFollower
{
    /*
     * Ludwig 3/19/2025: I wonder if a "stick" implementation would be interesting. As in, the carrot leads the entity
     * towards something, but the stick leads the entity away from something - say a projectile that could hit them.
     */
    public CatSpline Path;
    public Vector2 CarrotLocation;
    public float CarrotDistanceAlongPath;

    public void PlaceCarrotOnPath(Entity entity, int maxIterations = 5)
    {
        float closestDistance = float.MaxValue;

        for (int i = 0; i < maxIterations; i++)
        {
            float t = MathHelper.Lerp(0f, 1f, i / (maxIterations - 1f));
            Vector2 pathPos = Path.GetPos(t);
            float distance = Vector2.DistanceSquared(entity.EntityCenter, pathPos);
            
            if (distance < closestDistance)
            {
                closestDistance = distance;
            }
        }
    }

    public void PlaceCarrotAtStart()
    {
        CarrotLocation = Path.GetPos(0);
        CarrotDistanceAlongPath = 0f;
    }

    public void OrientMoverToCarrot(Entity entity, VelMover mover)
    {
        mover.TargetDirection = CarrotLocation - entity.EntityCenter;
    }

    public void MoveCarrotAlongPath(Entity entity, float leadDistance)
    {
        float distance = Vector2.Distance(entity.EntityCenter, CarrotLocation);
        if (distance < leadDistance)
        {
            CarrotDistanceAlongPath += leadDistance - distance;
            CarrotDistanceAlongPath = MathF.Min(Path.SplineLength, CarrotDistanceAlongPath);
        }

        CarrotLocation = Path.GetPos(Path.DistToT(CarrotDistanceAlongPath));
    }

    public bool CarrotAtEnd()
    {
        return Path.NotHasMinimumNumberOfControlPoints() || Math.Abs(CarrotDistanceAlongPath - Path.SplineLength) < 0.1f;
    }
}