using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FurrySharp.Entities.Base;
using FurrySharp.Maps;
using FurrySharp.Utilities;
using Microsoft.Xna.Framework;

namespace FurrySharp.Entities;

public struct EntityCastResult
{
    public Entity Entity;
    public bool WasHit => Entity != null;
    public CircleLineIntersectionResult CircleLineIntersection;
}

public class CollisionGroups
{
    public class Group
    {
        public readonly List<Entity> Colliders = new();
        public readonly List<Entity> Targets = new();
    }

    private readonly Dictionary<Type, Group> groups = new();
    private readonly List<Entity> mapColliders = new();
    private readonly List<Entity> mapEntities = new();
    private readonly List<Entity> raycastEntities = new();

    public void Register(Entity entity)
    {
        Type t = entity.GetType();

        // Iterate through all base types of the entity
        // Add the entity as a target for the colliders of the base types.
        for (Type d = t; d != typeof(Entity); d = d.BaseType)
        {
            Get(d).Targets.Add(entity);
        }

        IEnumerable<CollisionAttribute> cs = t.GetCustomAttributes<CollisionAttribute>().ToArray();

        if (cs.Any(c => c.MapCollider))
        {
            mapColliders.Add(entity);
        }

        if (cs.Any(c => c.MapEntity))
        {
            mapEntities.Add(entity);
        }

        if (cs.Any(c => c.RayCastable))
        {
            raycastEntities.Add(entity);
        }

        // Iterate through all collision attributes of the entity
        // Add the entity as a collider for the enumerated target types.
        foreach (Type target in cs.SelectMany(c => c.Targets))
        {
            Get(target).Colliders.Add(entity);
        }
    }

    public bool Unregister(Entity entity)
    {
        var result = mapColliders.Remove(entity);
        result = mapEntities.Remove(entity) || result;
        result = raycastEntities.Remove(entity) || result;
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
        if (map != null)
        {
            foreach (var entity in mapColliders)
            {
                map.Collide(entity);

                foreach (var m in mapEntities.Where(m => m.HitBox.Intersects(entity.HitBox)))
                {
                    m.Collided(entity);
                }
            }
        }

        foreach (Group g in groups.Values)
        {
            foreach (Entity collider in g.Colliders)
            {
                foreach (Entity target in g.Targets.Where(e => !ReferenceEquals(e, collider)))
                {
                    if (collider.HitBox.Intersects(target.HitBox))
                    {
                        collider.Collided(target);
                    }
                }
            }
        }
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

    public EntityCastResult RaycastForEntity<TTarget>(Vector2 start, Vector2 end, Entity excluding = null) where TTarget : Entity
    {
        float minT = float.MaxValue;
        Entity closest = null;
        CircleLineIntersectionResult circleLine = default;
        foreach (TTarget target in raycastEntities.OfType<TTarget>())
        {
            if (ReferenceEquals(target, excluding))
            {
                continue;
            }

            var result = CircleLineIntersection.Intersect(
                circleCenter: target.Position,
                radius: target.HitRadius,
                lineStart: start,
                lineEnd: end
            );

            if (result.Intersects && result.MinRealT < minT)
            {
                minT = result.MinRealT;
                closest = target;
                circleLine = result;
            }
        }

        return new EntityCastResult
        {
            Entity = closest,
            CircleLineIntersection = circleLine,
        };
    }
}