using System;
using Microsoft.Xna.Framework;

namespace FurrySharp.Entities.Base;

[Flags]
public enum Touching
{
    NONE = 0,
    LEFT = 1,
    RIGHT = 2,
    UP = 4,
    DOWN = 8,
    ANY = LEFT | RIGHT | UP | DOWN
}

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