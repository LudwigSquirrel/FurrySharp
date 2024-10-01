using FurrySharp.Entities;
using FurrySharp.Entities.Player;

namespace FurrySharp.States;

public class SandboxState : State
{
    public EntityManager EntityManager = new();

    public override void Create()
    {
        base.Create();
        EntityManager.Spawn<Player>();
    }

    public override void UpdateState()
    {
        EntityManager.UpdateEntities();
    }
    
    public override void DrawState()
    {
        EntityManager.DrawEntities();
    }
}