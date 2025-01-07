using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FurrySharp.Utilities;

public struct CircleLineIntersectionResult
{
    public float MinRealT;
    public List<Vector2> IntersectionPoints;
    public int Intersections => IntersectionPoints.Count;
    public bool Intersects => Intersections > 0;
}

public static class CircleLineIntersection
{
    public static CircleLineIntersectionResult Intersect(Vector2 circleCenter, float radius, Vector2 lineStart, Vector2 lineEnd)
    {
        CircleLineIntersectionResult result = new()
        {
            IntersectionPoints = new List<Vector2>(2)
        };

        float dx = lineEnd.X - lineStart.X;
        float dy = lineEnd.Y - lineStart.Y;
        float fx = lineStart.X - circleCenter.X;
        float fy = lineStart.Y - circleCenter.Y;

        // Quadratic equation coefficients.
        float a = dx * dx + dy * dy;
        float b = 2 * ((fx * dx) + (fy * dy));
        float c = fx * fx + fy * fy - radius * radius;
        
        // A negative discriminant tells us that there are no real values for t.
        float discriminant = b * b - 4 * a * c;
        if (discriminant < 0)
        {
            return result;
        }
        float sqrtdiscriminant = MathF.Sqrt(discriminant);
        float t1 = (-b - sqrtdiscriminant) / (2 * a);
        float t2 = (-b + sqrtdiscriminant) / (2 * a);

        // Check if the intersection points are on the line segment (between 0 and 1).
        if (t1 is >= 0 and <= 1)
        {
            result.MinRealT = t1;
            result.IntersectionPoints.Add(new Vector2(lineStart.X + t1 * dx, lineStart.Y + t1 * dy));
        }
        
        if (t2 is >= 0 and <= 1)
        {
            result.MinRealT = Math.Min(result.MinRealT, t2);
            result.IntersectionPoints.Add(new Vector2(lineStart.X + t2 * dx, lineStart.Y + t2 * dy));
        }

        return result;
    }
}