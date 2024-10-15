using System;

namespace FurrySharp.Entities.Base;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class CollisionAttribute : Attribute
{
    public CollisionAttribute(params Type[] targets)
    {
        Targets = targets;
    }

    // The types of entities that this entity collides with. This property doesn't have to be reciprocal (i.e. if A collides with B, B doesn't have to collide with A).
    public Type[] Targets { get; }

    // Whether this entity collides with the map.
    public bool MapCollider { get; set; }
    
    // Whether this entity is part of the map. Used for things like 'signs' or 'doors'.
    public bool MapEntity { get; set; }
    
    /* Ideas for expansion/optimization:
     * Add a property to specify that the entity is only collidable with entities that are on screen. Useful for player to enemy collisions.
     * Only calculate collisions using entities that have moved since the last frame.
     * Add a way to disable collision checking, and unregister self from collision groups. Useful for buttons that stay on after being pressed.
     * Spatially index entities. Create an array of Dictionary<Type, Group> and perform collision checks only on entities that are near each other.
     */
}