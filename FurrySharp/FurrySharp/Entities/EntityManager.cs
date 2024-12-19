﻿using System;
using System.Collections.Generic;
using FurrySharp.Drawing;
using FurrySharp.Entities.Base;
using FurrySharp.Logging;
using FurrySharp.Maps;
using FurrySharp.Registry;
using FurrySharp.Utilities;
using Microsoft.Xna.Framework;

namespace FurrySharp.Entities;

public class EntityManager
{
    private int idCounter;
    private List<Entity> onEntities = new(); // on screen, and being drawn
    private List<Entity> offEntities = new(); // off screen, and not being drawn
    private CollisionGroups collisionGroups = new();

    public MapInfo Map;

    // todo: implement a way to pass parameters for initialization to entities.
    // this overload is for when you want to spawn an entity that you know the type of.
    public void Spawn<TEntityType>(out TEntityType entity) where TEntityType : Entity, new()
    {
        entity = new TEntityType()
        {
            InstanceId = idCounter++,
            Manager = this,
        };
        onEntities.Add(entity); // todo: add to offEntities if off screen
        collisionGroups.Register(entity);
    }

    // this overload is for when you want to spawn an entity from a command or file.
    public bool Spawn(string name, out Entity entity)
    {
        try
        {
            Type type = EntityRegistry.GetEntityType(name);
            entity = (Entity)Activator.CreateInstance(type) ?? throw new Exception("Failed to create entity.");
        }
        catch (Exception e)
        {
            entity = null;
            DebugLogger.AddException(e);
            return false;
        }

        entity.InstanceId = idCounter++;
        entity.Manager = this;
        onEntities.Add(entity);
        collisionGroups.Register(entity);
        return true;
    }

    public void UpdateEntities()
    {
        foreach (Entity entity in onEntities)
        {
            entity.Update();
        }

        foreach (Entity entity in offEntities)
        {
            entity.Update();
        }
    }

    public void PostUpdateEntities()
    {
        foreach (Entity entity in onEntities)
        {
            entity.PostUpdate();
        }

        foreach (Entity entity in offEntities)
        {
            entity.PostUpdate();
        }
    }

    public void DoOnScreen()
    {
        for (var i = 0; i < onEntities.Count; i++)
        {
            Entity e = onEntities[i];
            Rectangle bb = e.BoundingBox;
            bb.Offset(e.Position);
            if (SpriteDrawer.Camera.Bounds.Intersects(bb) == false)
            {
                onEntities.RemoveAt(i);
                offEntities.Add(e);
            }
        }
        
        for (var i = 0; i < offEntities.Count; i++)
        {
            Entity e = offEntities[i];
            Rectangle bb = e.BoundingBox;
            bb.Offset(e.Position);
            if (SpriteDrawer.Camera.Bounds.Intersects(bb))
            {
                offEntities.RemoveAt(i);
                onEntities.Add(e);
            }
        }
    }

    public void DoCollisions()
    {
        collisionGroups.DoCollision(Map);
    }

    public void DrawEntities()
    {
        foreach (Entity entity in onEntities)
        {
            entity.Draw();
        }
    }

    public Entity GetEntity(int id)
    {
        return onEntities.Find(entity => entity.InstanceId == id) ?? offEntities.Find(entity => entity.InstanceId == id);
    }

    public bool RemoveEntity(Entity entity)
    {
        var result = onEntities.Remove(entity);
        result = result || offEntities.Remove(entity);
        result = result && collisionGroups.Unregister(entity);
        return result;
    }
    
    // public RayCastResult RayCast(Vector2 start, Vector2 end)
    // {
    //     // todo: first process with map, then with entities.
    // }
}