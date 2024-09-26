using FurrySharp.Entities.Player;

namespace FurrySharp.States;

public class SandboxState : State
{
    private Player player = new Player();

    public override void UpdateState()
    {
        player.UpdatePlayer();
    }
    
    public override void DrawState()
    {
        player.DrawPlayer();
    }
}