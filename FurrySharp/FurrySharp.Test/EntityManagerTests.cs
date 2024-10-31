using FurrySharp.Entities;
using FurrySharp.States;

namespace FurrySharp.Test;

public class EntityManagerTests
{
    [Test]
    public void EntityManagerTest()
    {
        var entityManager = new EntityManager();
        var result = entityManager.Spawn("Player", out _);
        Assert.True(result);
        result = entityManager.Spawn("player", out _);
        Assert.True(result);
        result = entityManager.Spawn("your mom", out _);
        Assert.False(result);
        var entity = entityManager.GetEntity(0);
        Assert.NotNull(entity);
        result = entityManager.RemoveEntity(entity);
        Assert.True(result);
        entity = entityManager.GetEntity(0);
        Assert.Null(entity);
        Assert.Pass();
    }
}