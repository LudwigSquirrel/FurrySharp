namespace FurrySharp.Drawing;

public enum DrawOrder
{
    BG,             // The background tilemap layer.
    Player,         // 
    Text,           // Text in dialogue.
    Fingy,          // The pointer used for UI interaction.
    BlackOverlay    // Black fadeout transition.
}

public class DrawingUtilities
{
    public static float GetDrawingZ(DrawOrder order)
    {
        float z = (float)order;
        return 1-z / (float)DrawOrder.BlackOverlay;
    }
}