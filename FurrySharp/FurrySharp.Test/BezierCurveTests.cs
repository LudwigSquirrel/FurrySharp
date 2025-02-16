using FurrySharp.Utilities;
using Microsoft.Xna.Framework;

namespace FurrySharp.Test;

public class BezierCurveTests
{
    const float LengthTolerance = 0.1f;
    const float PointTolerance = 1f; // 1 pixel

    [Test]
    public void TestSpline()
    {
        var spline = new BezierCurve();

        // Straight line
        spline.Segments.Add(new BezierCurveSegment()
        {
            A = new Vector2(0, 0),
            B = new Vector2(0, 0),
            C = new Vector2(0, 1),
            D = new Vector2(0, 1),
        });
        spline.UpdateLengthAndLookupTable();
        Assert.That(spline.SplineLength, Is.EqualTo(1f).Within(LengthTolerance));

        // Circle (approximation, don't at me)
        // see: https://stackoverflow.com/questions/1734745/how-to-create-circle-with-b%C3%A9zier-curves
        spline.Segments.Clear();
        var curve1 = new BezierCurveSegment()
        {
            A = new Vector2(1, 0),
            B = new Vector2(1, 0.552284749831f),
            C = new Vector2(0.552284749831f, 1),
            D = new Vector2(0, 1),
        };
        var curve2 = new BezierCurveSegment()
        {
            A = new Vector2(0, 1),
            B = new Vector2(-0.552284749831f, 1),
            C = new Vector2(-1, 0.552284749831f),
            D = new Vector2(-1, 0),
        };
        var curve3 = new BezierCurveSegment()
        {
            A = new Vector2(-1, 0),
            B = new Vector2(-1, -0.552284749831f),
            C = new Vector2(-0.552284749831f, -1),
            D = new Vector2(0, -1),
        };
        var curve4 = new BezierCurveSegment()
        {
            A = new Vector2(0, -1),
            B = new Vector2(0.552284749831f, -1),
            C = new Vector2(1, -0.552284749831f),
            D = new Vector2(1, 0),
        };
        spline.Segments.Add(curve1);
        spline.Segments.Add(curve2);
        spline.Segments.Add(curve3);
        spline.Segments.Add(curve4);
        spline.UpdateLengthAndLookupTable();
        Assert.That(spline.SplineLength, Is.EqualTo(6.2831853071796f).Within(LengthTolerance));

        var points = spline.GetEvenlySpacedPoints(360);
        var expected = MathUtilities.PlotCircle(Vector2.Zero, 1, 360);

        for (var i = 0; i < points.Length; i++)
        {
            AssertPointHelper(points[i], expected[i]);
        }

        void AssertPointHelper(Vector2 point, Vector2 expected)
        {
            Assert.That(point.X, Is.EqualTo(expected.X).Within(PointTolerance));
            Assert.That(point.Y, Is.EqualTo(expected.Y).Within(PointTolerance));
        }
    }
}