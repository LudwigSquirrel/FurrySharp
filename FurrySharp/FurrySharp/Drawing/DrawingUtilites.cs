namespace FurrySharp.Drawing;

public enum DrawOrder
{
    Front,          //
    Text,           // Text in dialogue.
    BlackOverlay    // Black fadeout transition
}

public class DrawingUtilites
{
    public static float GetDrawingZ(DrawOrder order)
    {
        float z = (float)order;
        return 1-z / (float)DrawOrder.BlackOverlay;
    }
}