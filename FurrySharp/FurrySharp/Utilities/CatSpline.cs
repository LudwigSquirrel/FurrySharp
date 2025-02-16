using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace FurrySharp.Utilities;

public class CatSpline
{
    public bool Loop = false;
    public List<Vector2> ControlPoints = new List<Vector2>();

    public float SplineLength => LUT.Count > 0 ? LUT[^1] : 0;

    [JsonIgnore]
    public List<float> LUT = new();

    public void UpdateLengthAndLookupTable(int samples = 100) // estimated length
    {
        if (NotHasMinimumNumberOfControlPoints())
        {
            return;
        }

        float step = 1f / samples;
        float length = 0f;
        LUT.Clear();
        LUT.Add(0f);
        for (int i = 0; i < ControlPoints.Count - (Loop ? 0 : 3); i++)
        {
            for (float t = 0; t < 1; t += step)
            {
                Vector2 p0 = GetPos(i + t);
                Vector2 p1 = GetPos(i + t + step);
                length += Vector2.Distance(p0, p1);
                LUT.Add(length);
            }
        }
    }
    
    public Vector2[] GetEvenlySpacedPoints(int numPoints, float start = 0f, float end = 1f)
    {
        if (NotHasMinimumNumberOfControlPoints())
        {
            return new[] { Vector2.Zero };
        }

        Vector2[] points = new Vector2[numPoints];
        float current = TToDist(start);
        float span = (TToDist(end) - current) / numPoints;

        for (var i = 0; i < points.Length; i++)
        {
            points[i] = GetPos(DistToT(current));
            current += span;
        }

        return points;
    }

    public float TToDist(float t)
    {
        int n = LUT.Count;
        t = Math.Clamp(t, 0, ControlPoints.Count - (Loop ? 0 : 3));
        int i1 = (int)(n * t);
        int i2 = i1 + 1;
        if (i2 >= n)
        {
            return SplineLength;
        }

        float foo = (t - (float)i1 / n) * n;
        return MathHelper.Lerp(LUT[i1], LUT[i2], foo);
    }

    public float DistToT(float distance)
    {
        int n = LUT.Count;

        if (distance > SplineLength)
        {
            if (Loop)
            {
                distance %= SplineLength;
            }
            else
            {
                return 1;
            }
        }

        if (distance <= 0)
        {
            if (Loop)
            {
                distance = (distance % SplineLength + SplineLength);
            }
            else
            {
                return 0;
            }
        }

        for (int i = 0; i < n - 1; i++)
        {
            if (distance >= LUT[i] && distance <= LUT[i + 1])
            {
                return MathUtilities.Remap(distance, LUT[i], LUT[i + 1], (float)i / n, (float)(i + 1) / n) * ControlPoints.Count - (Loop ? 0 : 3);
            }
        }
        
        /*
         * Ludwig 12/12/2024:
         * I've had this thrown a few times in testing. I'm hoping that the '>=' and '<=' (see above) will fix it. If
         * not, I'll have to make a unit test to reproduce the issue better.
         */
        var e = new ApplicationException("There was no spot in the LUT that the distance seemed to fall between.");
        e.Data.Add("distance", distance);
        e.Data.Add("LUT Count", LUT.Count);
        for (var i = 0; i < LUT.Count; i++)
        {
            e.Data.Add($"LUT[{i}]", LUT[i]);
        }

        throw e;
    }

    public Vector2 GetPos(float t)
    {
        if (NotHasMinimumNumberOfControlPoints())
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

    public bool NotHasMinimumNumberOfControlPoints()
    {
        return (!Loop && ControlPoints.Count < 4) || (Loop && ControlPoints.Count < 3);
    }
}