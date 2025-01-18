using System;
using FurrySharp.Multiplatform;
using FurrySharp.States;
using static ImGuiNET.ImGui;

namespace FurrySharp.DeveloperTools;

public static class DevTools
{
    public static void DoIMGUI()
    {
        var currentState = Program.FurryGame.CurrentState;
        if (BeginMainMenuBar())
        {
            if (BeginMenu("Windows"))
            {
                if (MenuItem($"{currentState.GetType().Name}"))
                {
                    currentState.ShowIMGUI = !currentState.ShowIMGUI;
                }
                EndMenu();
            }
            if (BeginMenu("State"))
            {
                Type[] stateTypes = new []{typeof(SandboxState), typeof(CatSplineEditorState)};
                foreach (var stateType in stateTypes)
                {
                    if (MenuItem(stateType.Name))
                    {
                        Program.FurryGame.CreateAndSetState(stateType);
                    }
                }
                EndMenu();
            }

            if (currentState.ShowIMGUI)
            {
                currentState.DoIMGUIMainMenuBar();
            }
            EndMainMenuBar();
        }
    }
}