using System;

namespace FurrySharp.Entities.Enemy;

[AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public class EnemyAttribute : Attribute
{
    public EnemyAttribute()
    {
    }
}