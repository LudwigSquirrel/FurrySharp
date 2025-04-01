namespace FurrySharp.Entities.Components;

public class TickyTimer
{
    private int timer = 0;

    public bool Loop { get; set; }
    public int FrameInterval { get; set; }

    public float SecondInterval
    {
        get => FrameInterval / 60f;
        set => FrameInterval = (int)(60 * value);
    }

    public bool DoTick()
    {
        if ( timer < FrameInterval)
        {
            timer++;
            return false;
        }
        else
        {
            if (Loop) Reset();
            return true;
        }
    }

    public void Reset()
    {
        timer = 0;
    }
}