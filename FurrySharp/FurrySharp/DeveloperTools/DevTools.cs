using System;
using System.Diagnostics;
using FurrySharp.Multiplatform;
using FurrySharp.Resources;
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

                if (MenuItem("Open auto save folder"))
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = @"c:\windows\explorer.exe",
                        Arguments = ResourceInstanceAutoSaver.AutoSaveDir.TrimEnd('/')
                    };
                    Process.Start(startInfo);
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