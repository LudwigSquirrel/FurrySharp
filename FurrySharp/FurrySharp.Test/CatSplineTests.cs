using FurrySharp.Utilities;
using Microsoft.Xna.Framework;

namespace FurrySharp.Test;

public class CatSplineTests
{
    const float LengthTolerance = 0.1f;
    const float PointTolerance = 1f; // 1 pixel

    [Test]
    public void TestSpline()
    {
        var spline = new CatSpline();
        Assert.That(spline.NotHasMinimumNumberOfControlPoints, Is.True);
        Assert.That(spline.SplineLength, Is.EqualTo(0f));
        Assert.That(spline.GetPos(1), Is.EqualTo(Vector2.Zero));
        Assert.That(spline.TToDist(1), Is.EqualTo(0f));
        Assert.That(spline.DistToT(1), Is.EqualTo(0f));
        Assert.That(spline.GetEvenlySpacedPoints(1), Is.EqualTo(new[] { Vector2.Zero }));

        // Straight line
        spline.ControlPoints.Add(new Vector2(0, -1));
        spline.ControlPoints.Add(new Vector2(0, 0));
        spline.ControlPoints.Add(new Vector2(0, 1));
        spline.ControlPoints.Add(new Vector2(0, 2));
        spline.UpdateLengthAndLookupTable();
        Assert.That(spline.SplineLength, Is.EqualTo(1f).Within(LengthTolerance));
        
        // Circle.
        spline.ControlPoints.Clear();
        spline.Loop = true;
        var points = MathUtilities.PlotCircle(Vector2.Zero, 1, 90);
        spline.ControlPoints.AddRange(points);
        spline.UpdateLengthAndLookupTable();
        Assert.That(spline.SplineLength, Is.EqualTo(MathUtilities.Circumference(1)).Within(LengthTolerance));
        var evenlySpacedPoints = spline.GetEvenlySpacedPoints(360);
        var expectedPoints = MathUtilities.PlotCircle(Vector2.Zero, 1, 360);
        for (int i = 0; i < points.Length; i++)
        {
            AssertPointHelper(evenlySpacedPoints[i], expectedPoints[i]);
        }
        void AssertPointHelper(Vector2 point, Vector2 expected)
        {
            Assert.That(point.X, Is.EqualTo(expected.X).Within(PointTolerance));
            Assert.That(point.Y, Is.EqualTo(expected.Y).Within(PointTolerance));
        }
    }
}