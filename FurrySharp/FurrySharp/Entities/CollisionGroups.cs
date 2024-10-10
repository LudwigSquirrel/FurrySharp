﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FurrySharp.Entities.Base;
using FurrySharp.Maps;

namespace FurrySharp.Entities;

public class CollisionGroups
{
    public class Group
    {
        public readonly List<Entity> Colliders = new();
        public readonly List<Entity> Targets = new();
    }

    private readonly Dictionary<Type, Group> groups = new();
    private readonly List<Entity> mapColliders = new();

    public void Register(Entity entity)
    {
        Type t = entity.GetType();
        
        // Iterate through all base types of the entity
        // Add the entity as a target for the colliders of the base types.
        for(Type d = t; d != typeof(Entity); d = d.BaseType)
        {
            Get(d).Targets.Add(entity);
        }

        IEnumerable<CollisionAttribute> cs = t.GetCustomAttributes<CollisionAttribute>().ToArray();

        if (cs.Any(c => c.MapCollision))
        {
            mapColliders.Add(entity);
        }

        // Iterate through all collision attributes of the entity
        // Add the entity as a collider for the enumerated target types.
        foreach(Type target in cs.SelectMany(c=>c.Types))
        {
            Get(target).Colliders.Add(entity);
        }
    }

    public bool Unregister(Entity entity)
    {
        var result = mapColliders.Remove(entity);
        foreach (var pair in groups)
        {
            /*
             * Ludwig 10/9/2024: I discovered through unit testing that this code, although seemingly identical to the
             * correct version, causes a glitch when 'result' is set to true. This is because in an OR operation, if the
             * lhand condition is already true, then the rhand won't process. How cool is that!
             */
            // result = result || pair.Value.Colliders.Remove(entity);
            // result = result || pair.Value.Targets.Remove(entity);
            result = pair.Value.Colliders.Remove(entity) || result;
            result = pair.Value.Targets.Remove(entity) || result;
        }
        
        return result;
    }

    public void DoCollision(MapInfo map)
    {
        // todo: implement collision detection.
    }

    public Group Get(Type t)
    {
        if (!groups.TryGetValue(t, out Group g))
        {
            g = new Group();
            groups.Add(t, g);
        }

        return g;
    }
}