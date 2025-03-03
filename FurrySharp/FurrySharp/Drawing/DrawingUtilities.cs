using System;

namespace FurrySharp.Drawing;

public enum DrawOrder
{
    BG, // The background tilemap layer.
    EntitiesBG, //
    Entities, // Enemies, the player, etc.
    EntitiesFG, //
    TerminalBG, //
    Text, // Text in dialogue.
    Fingy, // The pointer used for UI interaction.
    DebugLine, // Lines drawn for debug purposes.
    BlackOverlay // Black fadeout transition.
}

public class DrawingUtilities
{
    public static float GetDrawingZ(DrawOrder order, float y, float yMax = 100f)
    {
        float z = (float)order;
        float normY = Math.Clamp(y / yMax, 0f, 0.99f);
        return 1 - (z + normY) / (float)DrawOrder.BlackOverlay;
    }

    public static float GetDrawingZ(DrawOrder order)
    {
        float z = (float)order;
        return 1 - z / (float)DrawOrder.BlackOverlay;
    }
}