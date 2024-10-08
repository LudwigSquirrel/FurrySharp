namespace FurrySharp.Drawing;

public enum DrawOrder
{
    BG,             // The background tilemap layer.
    Front,          //
    Text,           // Text in dialogue.
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