using Microsoft.Xna.Framework;

namespace FurrySharp.Entities.Base;

public class Entity
{
    public int InstanceId;
    public Vector2 Position;
    
    public virtual void Update()
    {
        // Do nothing
    }
    
    public virtual void Draw()
    {
        // Do nothing
    }
}