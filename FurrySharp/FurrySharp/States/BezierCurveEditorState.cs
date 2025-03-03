using FurrySharp.Drawing;
using FurrySharp.Utilities;
using ImGuiNET;
using Microsoft.Xna.Framework;
using MonoGame.ImGuiNet.Extensions;

namespace FurrySharp.States;

// Ludwig 1/8/2025: Bézier curves are actually a little too complex for my use case. I'm thinking I'll use splines instead.
public class BezierCurveEditorState : State
{
    public string SplineName = "";
    public int Samples = 100;
    public int Resolution = 10;
    public BezierCurve CurrentBezierCurve = new BezierCurve();

    public override void Create()
    {
        base.Create();
        SpriteDrawer.Camera.CenterOn(Vector2.Zero);
    }

    public override void DrawState()
    {
        base.DrawState();
        CurrentBezierCurve.UpdateLengthAndLookupTable(Samples);
        SpriteDrawer.DrawLines(CurrentBezierCurve.GetEvenlySpacedPoints(Resolution), Color.White, false, DrawingUtilities.GetDrawingZ(DrawOrder.DebugLine));
    }

    public override void DoIMGUI()
    {
        base.DoIMGUI();
        ImGui.Begin("Spline Editor");
        ImGui.InputText("Spline Name", ref SplineName, 100);
        ImGui.DragInt("Samples", ref Samples, v_min: 1, v_max: 100, v_speed: 0);
        ImGui.DragInt("Resolution", ref Resolution, v_min: 1, v_max: 10, v_speed: 0);
        for (var i = 0; i < CurrentBezierCurve.Segments.Count; i++)
        {
            var curve = CurrentBezierCurve.Segments[i];
            DeerGui.DragFloat2(nameof(BezierCurveSegment.A) + i, ref CurrentBezierCurve.Segments[i].A);
            DeerGui.DragFloat2(nameof(BezierCurveSegment.B) + i, ref CurrentBezierCurve.Segments[i].B);
            DeerGui.DragFloat2(nameof(BezierCurveSegment.C) + i, ref CurrentBezierCurve.Segments[i].C);
            DeerGui.DragFloat2(nameof(BezierCurveSegment.D) + i, ref CurrentBezierCurve.Segments[i].D);
        }

        if (ImGui.Button("Add Segment"))
        {
            var item = new BezierCurveSegment();
            if (CurrentBezierCurve.Segments.Count > 0)
            {
                item.A = item.B = item.C = item.D = CurrentBezierCurve.Segments[CurrentBezierCurve.Segments.Count - 1].D;
            }
            CurrentBezierCurve.Segments.Add(item);
        }
        
        ImGui.End();
    }
}