using FurrySharp.Multiplatform;
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
            EndMainMenuBar();
        }
    }
}