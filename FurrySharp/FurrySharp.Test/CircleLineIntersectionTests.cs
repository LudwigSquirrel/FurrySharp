using FurrySharp.Utilities;
using Microsoft.Xna.Framework;

namespace FurrySharp.Test;

public static class CircleLineIntersectionTests
{
    [TestCase(-10, 0, 1, 0, 0, 1, 0, false, 0)]
    [TestCase(0, 0, 1, 0, 0, 1, 0, true, 1, 1f, 0f)]
    [TestCase(0, 0, 1, -1, 0, 0, 1, true, 2, -1, 0, 0, 1)]
    public static void TestCircleLineIntersection(float cx, float cy, float r, float x1, float y1, float x2, float y2, bool intersects, int intersections, params float[] points)
    {
        var circleCenter = new Vector2(cx, cy);
        var radius = r;
        var lineStart = new Vector2(x1, y1);
        var lineEnd = new Vector2(x2, y2);
        var result = CircleLineIntersection.Intersect(circleCenter, radius, lineStart, lineEnd);
        Assert.That(result.Intersects, Is.EqualTo(intersects));
        Assert.That(result.Intersections, Is.EqualTo(intersections));
        for (int i = 0; i < points.Length; i += 2)
        {
            Assert.That(result.IntersectionPoints[i / 2], Is.EqualTo(new Vector2(points[i], points[i + 1])));
        }
    }
}