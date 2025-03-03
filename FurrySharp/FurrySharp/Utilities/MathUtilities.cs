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

    public static Vector2[] PlotCircle(Vector2 center, float radius, int numPoints, float degreeOffset = 0)
    {
        Vector2[] ret = new Vector2[numPoints];
        float step = MathHelper.TwoPi / numPoints;
        float offset = MathHelper.ToRadians(degreeOffset);
        for (int i = 0; i < numPoints; i++)
        {
            float angle = offset + (step * i);
            float x = center.X + radius * (float)Math.Cos(angle);
            float y = center.Y + radius * (float)Math.Sin(angle);
            ret[i] = new Vector2(x, y);
        }

        return ret;
    }
    
    public static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static float Clamp(float f, float min, float max)
    {
        return MathF.Max(min, MathF.Min(f, max));
    }

    public static float Circumference(float radius)
    {
        return MathF.PI * 2 * radius;
    }

    public static float TriangleWave(float t, float amplitude)
    {
        return MathF.Abs((t % (amplitude * 2)) - (amplitude));
    }
}