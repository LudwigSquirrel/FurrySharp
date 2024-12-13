using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace FurrySharp.Utilities;

public class BezierCurve
{
    public Vector2 A;
    public Vector2 B;
    public Vector2 C;
    public Vector2 D;

    public Vector2 GetPos(float t)
    {
        Vector2 p0 = Vector2.Lerp(A, B, t);
        Vector2 p1 = Vector2.Lerp(B, C, t);
        Vector2 p2 = Vector2.Lerp(C, D, t);
        Vector2 p3 = Vector2.Lerp(p0, p1, t);
        Vector2 p4 = Vector2.Lerp(p1, p2, t);
        return Vector2.Lerp(p3, p4, t);
    }
}

public class Spline
{
    [JsonInclude]
    public List<BezierCurve> Segments = new();

    public float SplineLength => LUT[^1];

    public List<float> LUT = new();

    public void UpdateLengthAndLookupTable(int samples = 100) // estimated length
    {
        float step = 1f / samples;
        float length = 0f;
        LUT.Clear();
        LUT.Add(0f);
        for (int i = 0; i < Segments.Count; i++)
        {
            BezierCurve segment = Segments[i];
            for (float t = 0; t < 1; t += step)
            {
                Vector2 p0 = segment.GetPos(t);
                Vector2 p1 = segment.GetPos(t + step);
                length += Vector2.Distance(p0, p1);
                LUT.Add(length);
            }
        }
    }

    public Vector2[] GetEvenlySpacedPoints(int numPoints, float start = 0f, float end = 1f)
    {
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

    public float DistToT(float distance)
    {
        int n = LUT.Count;

        if (distance > SplineLength)
        {
            return 1;
        }

        if (distance <= 0)
        {
            return 0;
        }

        for (int i = 0; i < n - 1; i++)
        {
            if (distance >= LUT[i] && distance <= LUT[i + 1])
            {
                return MathUtilities.Remap(distance, LUT[i], LUT[i + 1], (float)i / n, (float)(i + 1) / n);
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

    public float TToDist(float t)
    {
        int n = LUT.Count;
        t = Math.Clamp(t, 0, 1);
        int i1 = (int)(n * t);
        int i2 = i1 + 1;
        if (i2 >= n)
        {
            return SplineLength;
        }
        
        float foo = (t - (float)i1 / n) * n;
        return MathHelper.Lerp(LUT[i1], LUT[i2], foo);
    }

    public Vector2 GetPos(float t)
    {
        t = Math.Clamp(t, 0, 1);
        int i = (int)((Segments.Count - 1) * t);
        BezierCurve seg = Segments[i];

        var segSpan = 1f / Segments.Count;
        var bar = segSpan * i;
        t = (t - bar) / segSpan;

        return seg.GetPos(t);
    }
}