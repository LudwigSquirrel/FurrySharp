using System;

namespace FurrySharp.Animation;

public static class AnimUtility
{
   // The variable x represents the absolute progress of the animation in the bounds of 0 (beginning of the animation) and 1 (end of animation).
    public static float EaseInSine(float x)
    {
        return 1 - MathF.Cos(x * MathF.PI / 2);
    }

    // Ditto.
    public static float EaseOutSine(float x)
    {
        return MathF.Sin(x * MathF.PI / 2);
    }

    public static float EaseInOutSine(float x)
    {
        return -(MathF.Cos(MathF.PI * x) - 1) / 2;
    }
}