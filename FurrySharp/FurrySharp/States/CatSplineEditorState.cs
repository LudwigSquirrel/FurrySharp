﻿using System;
using System.Collections.Generic;
using System.Globalization;
using FurrySharp.Drawing;
using FurrySharp.Entities.Components;
using FurrySharp.Input;
using FurrySharp.Registry;
using FurrySharp.Utilities;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.ImGuiNet.Extensions;

namespace FurrySharp.States;

public class CatSplineEditorState : State
{
    public string SplineName = "My Spine";
    public List<int> SelectedPoints = new List<int>();
    public CatSpline CatSpline = new CatSpline();
    public VelMover Mover = new VelMover();

    public float MoveSpeed = 96f;
    public float BumpAmount = 1f;
    public float Resolution = 0.06f;
    public float Zoom = 1f;

    public override void Create()
    {
        base.Create();
        GlobalState.ShowDevTools = true;
        SpriteDrawer.Camera.CenterOn(Vector2.Zero);
        CatSpline.ControlPoints.Add(new Vector2(0, 0));
        CatSpline.ControlPoints.Add(new Vector2(100, 0));
        CatSpline.ControlPoints.Add(new Vector2(100, 100));
        CatSpline.ControlPoints.Add(new Vector2(0, 100));
    }

    public override void UpdateState()
    {
        base.UpdateState();
        Vector2 trans = Vector2.Zero;
        Mover.TargetSpeed = MoveSpeed;
        Mover.TargetDirection = Vector2.Zero;
        if (GameInput.IsKeyPressed(Keys.W))
        {
            Mover.TargetDirection.Y -= 1f;
        }

        if (GameInput.IsKeyPressed(Keys.S))
        {
            Mover.TargetDirection.Y += 1f;
        }

        if (GameInput.IsKeyPressed(Keys.A))
        {
            Mover.TargetDirection.X -= 1f;
        }

        if (GameInput.IsKeyPressed(Keys.D))
        {
            Mover.TargetDirection.X += 1f;
        }

        if (GameInput.JustPressedKey(Keys.Up))
        {
            trans.Y -= BumpAmount;
        }

        if (GameInput.JustPressedKey(Keys.Down))
        {
            trans.Y += BumpAmount;
        }

        if (GameInput.JustPressedKey(Keys.Left))
        {
            trans.X -= BumpAmount;
        }

        if (GameInput.JustPressedKey(Keys.Right))
        {
            trans.X += BumpAmount;
        }

        Mover.DoVel();
        trans += Mover.Velocity * GameTimes.DeltaTime;

        for (var i = 0; i < CatSpline.ControlPoints.Count; i++)
        {
            if (SelectedPoints.Contains(i))
            {
                CatSpline.ControlPoints[i] += trans;
            }
        }
    }

    public override void DrawState()
    {
        base.DrawState();
        for (float t = 0; t <= CatSpline.ControlPoints.Count; t += Resolution)
        {
            Vector2 start = CatSpline.GetPoint(t) * Zoom;
            Vector2 end = CatSpline.GetPoint(t + Resolution) * Zoom;
            Color color = Color.Lerp(Color.MonoGameOrange, Color.Black, MathF.Sin((t - GameTimes.TotalTime) * 4f));
            SpriteDrawer.DrawLine(start, end, color);
        }

        for (var i = 0; i < CatSpline.ControlPoints.Count; i++)
        {
            var point = CatSpline.ControlPoints[i] * Zoom;
            Vector2[] points = MathUtilities.PlotCircle(point, 4, 4);
            SpriteDrawer.DrawLines(points, SelectedPoints.Contains(i) ? Color.White : Color.Gray);
        }
    }

    public override void DoIMGUI()
    {
        base.DoIMGUI();

        ImGui.Begin("CatSpline Editor");
        ImGui.InputText("Spline Name", ref SplineName, 100);
        ImGui.Checkbox("Loop", ref CatSpline.Loop);
        ImGui.DragFloat("Move Speed", ref MoveSpeed, 0.01f, 0.1f, 10f, "%.2f");
        ImGui.DragFloat("Bump Amount", ref BumpAmount, 0.01f, 0.1f, 1f, "%.2f");
        ImGui.DragFloat("Resolution", ref Resolution, 0.01f, 0.01f, 1f, "%.2f");
        ImGui.DragFloat("Zoom", ref Zoom, 0.01f, 0.1f, 10f, "%.2f");
        ImGui.End();

        ImGui.Begin("Control Points");
        for (int i = 0; i < CatSpline.ControlPoints.Count; i++)
        {
            bool selected = SelectedPoints.Contains(i);
            if (ImGui.Checkbox($"{i}", ref selected))
            {
                if (selected)
                {
                    SelectedPoints.Add(i);
                }
                else
                {
                    SelectedPoints.Remove(i);
                }
            }
            ImGui.SameLine();
            var catSplineControlPoint = CatSpline.ControlPoints[i];
            DeerGui.DragFloat2($"Position##{i}", ref catSplineControlPoint);
            CatSpline.ControlPoints[i] = catSplineControlPoint;
            ImGui.SameLine();
            if (ImGui.Button($"Insert##{i}"))
            {
                CatSpline.ControlPoints.Insert(i, CatSpline.ControlPoints[i]);
            }
            ImGui.SameLine();
            if (ImGui.Button($"Delete##{i}"))
            {
                CatSpline.ControlPoints.RemoveAt(i);
            }
        }

        if (ImGui.Button("Add"))
        {
            CatSpline.ControlPoints.Add(Vector2.Zero);
        }
        ImGui.End();
    }
}