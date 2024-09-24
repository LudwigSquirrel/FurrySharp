using System;

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
}