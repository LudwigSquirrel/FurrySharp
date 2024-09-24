using System;
using System.Collections.Generic;

namespace FurrySharp.Utilities;

public class ObjectPool<T>
{
    private readonly Func<T> spawner;
    private readonly List<T> values = new();
    private int currentIndex = 0;
    
    public ObjectPool(Func<T> spawner)
    {
        this.spawner = spawner;
    }

    public T GetNext()
    {
        if(values.Count == currentIndex)
        {
            values.Add(spawner());
        }
        return values[currentIndex++];
    }

    public void Reset()
    {
        currentIndex = 0;
    }
}