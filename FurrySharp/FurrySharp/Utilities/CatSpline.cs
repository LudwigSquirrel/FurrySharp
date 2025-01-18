using System;
using System.Collections.Generic;
using FurrySharp.Drawing;
using Microsoft.Xna.Framework;

namespace FurrySharp.Utilities;

public class CatSpline
{
    public bool Loop = false;
    public List<Vector2> ControlPoints = new List<Vector2>();

    public Vector2 GetPoint(float t)
    {
        if ((!Loop && ControlPoints.Count < 4) || (Loop && ControlPoints.Count < 3))
        {
            return Vector2.Zero;
        }

        if (Loop)
        {
            t %= ControlPoints.Count;
        }
        float tClamped = MathUtilities.Clamp(t, 0, ControlPoints.Count - (Loop ? 0.01f : 3.01f));

        // Catmull-Rom spline algorithm
        int p0, p1, p2, p3;
        if (Loop)
        {
            p1 = (int)tClamped;
            p2 = (p1 + 1) % ControlPoints.Count;
            p3 = (p2 + 1) % ControlPoints.Count;
            p0 = (p1 - 1) < 0 ? ControlPoints.Count - 1 : p1 - 1;
        }
        else
        {
            p1 = (int)tClamped + 1;
            p2 = (p1 + 1);
            p3 = (p2 + 1);
            p0 = p1 - 1;
        }

        float tNorm = tClamped - (int)tClamped;

        float tt = tNorm * tNorm;
        float ttt = tt * tNorm;

        float q1 = -ttt + 2.0f * tt - tNorm;
        float q2 = 3.0f * ttt - 5.0f * tt + 2.0f;
        float q3 = -3.0f * ttt + 4.0f * tt + tNorm;
        float q4 = ttt - tt;

        float tx = 0.5f * ((ControlPoints[p0].X * q1) + (ControlPoints[p1].X * q2) + (ControlPoints[p2].X * q3) + (ControlPoints[p3].X * q4));
        float ty = 0.5f * ((ControlPoints[p0].Y * q1) + (ControlPoints[p1].Y * q2) + (ControlPoints[p2].Y * q3) + (ControlPoints[p3].Y * q4));
        return new Vector2(tx, ty);
    }
}