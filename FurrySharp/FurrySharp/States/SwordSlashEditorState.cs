using FurrySharp.Resources;
using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;

namespace FurrySharp.States;

public class SwordSlashEditorState : State
{
    private Texture Ludwig;
    private CatSplineEditorState CatSplineEditorState;
    
    public float Zoom = 1f;
    public string LoadedSpline = "slash";
    
    public bool EditingSpline = false;
    
    public override void Create()
    {
        base.Create();
        Ludwig = ResourceManager.GetTexture("ludwig_player");
        CatSplineEditorState = new CatSplineEditorState();
        CatSplineEditorState.Create();
        
        CatSplineEditorState.CatSpline = ResourceManager.GetSpline(LoadedSpline);
        CatSplineEditorState.SplineName = LoadedSpline;
    }

    public override void UpdateState()
    {
        if (EditingSpline)
        {
            CatSplineEditorState.UpdateState();
            return;
        }
    }
    
    public override void DrawState()
    {
        if (EditingSpline)
        {
            CatSplineEditorState.DrawState();
            return;
        }
    }

    public override void DoIMGUI()
    {
        if (EditingSpline)
        {
            CatSplineEditorState.DoIMGUI();
            return;
        }
        
        ImGui.Begin("Sword Slash Editor");
        if (ImGui.BeginCombo("Spline to Edit", LoadedSpline))
        {
            foreach (var spline in ResourceManager.GetSplineKeys())
            {
                if (ImGui.Selectable(spline))
                {
                    LoadedSpline = spline;
                    CatSplineEditorState.CatSpline = ResourceManager.GetSpline(LoadedSpline);
                    CatSplineEditorState.SplineName = LoadedSpline;
                }
            }
            ImGui.EndCombo();
        }
        
        if (ImGui.Button("Edit Spline"))
        {
            EditingSpline = true;
        }
        ImGui.End();
    }

    public override void DoIMGUIMainMenuBar()
    {
        if (EditingSpline)
        {
            CatSplineEditorState.DoIMGUIMainMenuBar();
            EditingSpline = !ImGui.Button("Return to Sword Slash Editor");
            return;
        }
    }
}