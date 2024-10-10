using System;

namespace FurrySharp.Entities.Base;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class CollisionAttribute : Attribute
{
    public CollisionAttribute(params Type[] types)
    {
        Types = types;
    }

    public Type[] Types { get; }

    public bool MapCollision { get; set; }
}