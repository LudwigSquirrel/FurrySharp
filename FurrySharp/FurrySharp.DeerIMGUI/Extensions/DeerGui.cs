using ImGuiNET;
using Microsoft.Xna.Framework;

namespace MonoGame.ImGuiNet.Extensions;

public static class DeerGui
{
    public static bool DragFloat2(string label, ref Vector2 xnaVec)
    {
        var numeric = xnaVec.ToNumerics();
        bool result = ImGui.DragFloat2(label, ref numeric);
        xnaVec = numeric;
        return result;
    }
}