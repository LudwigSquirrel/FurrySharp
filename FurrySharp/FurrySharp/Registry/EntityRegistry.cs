using System;
using System.Collections.Generic;
using System.Reflection;
using FurrySharp.Entities.Base;

namespace FurrySharp.Registry;

// Class to store all classes that inherit from Entity, and make spawning them easier.
public static class EntityRegistry
{
    private static Dictionary<string, Type> entityTypes = new();
    
    static EntityRegistry()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        foreach (Type type in assembly.GetTypes())
        {
            if (type.IsSubclassOf(typeof(Entity)))
            {
                entityTypes.Add(type.Name.ToLower(), type);
            }
        }
    }
    
    public static Type GetEntityType(string name)
    {
        name = name.ToLower();
        return entityTypes[name];
    }
}