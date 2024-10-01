﻿using System;
using System.Collections.Generic;
using FurrySharp.Entities.Base;
using FurrySharp.Logging;
using FurrySharp.Registry;

namespace FurrySharp.Entities;

public class EntityManager
{
    private int idCounter;
    private List<Entity> onEntities = new(); // on screen, and being drawn
    private List<Entity> offEntities = new(); // off screen, and not being drawn

    // todo: implement a way to pass parameters for initialization to entities.
    // this overload is for when you want to spawn an entity that you know the type of.
    public void Spawn<TEntityType>() where TEntityType : Entity, new()
    {
        onEntities.Add(new TEntityType()
        {
            InstanceId = idCounter++,
        });
    }

    // this overload is for when you want to spawn an entity from a command or file.
    public bool Spawn(string name)
    {
        Entity entity;
        try
        {
            Type type = EntityRegistry.GetEntityType(name);
            entity = (Entity)Activator.CreateInstance(type) ?? throw new Exception("Failed to create entity.");
        }
        catch (Exception e)
        {
            DebugLogger.AddException(e);
            return false;
        }

        entity.InstanceId = idCounter++;
        onEntities.Add(entity);
        return true;
    }

    public void UpdateEntities()
    {
        // Assume I implemented a update method in the Entity class
        foreach (Entity entity in onEntities)
        {
            entity.Update();
        }

        foreach (Entity entity in offEntities)
        {
            entity.Update();
        }
        
        // todo: implement a way to move entities between onEntities and offEntities.
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
        return result;
    }
}