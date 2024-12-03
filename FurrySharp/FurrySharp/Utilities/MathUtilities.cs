using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FurrySharp.Utilities;

public class MathUtilities
{
    public static bool MoveTo(ref float v, float target, float speed)
    {
        if (v > target)
        {
            v = Math.Max(target, v - speed * GameTimes.DeltaTime);
        }
        else
        {
            v = Math.Min(target, v + speed * GameTimes.DeltaTime);
        }

        return v == target;
    }

    public static Vector2[] PlotCircle(Vector2 center, float radius, int numPoints, float offset = 0)
    {
        Vector2[] ret = new Vector2[numPoints];
        for (int i = 0; i < numPoints; i++)
        {
            float angle = offset + (MathHelper.TwoPi * i / numPoints);
            float x = center.X + radius * (float)Math.Cos(angle);
            float y = center.Y + radius * (float)Math.Sin(angle);
            ret[i] = new Vector2(x, y);
        }

        return ret;
    }
}