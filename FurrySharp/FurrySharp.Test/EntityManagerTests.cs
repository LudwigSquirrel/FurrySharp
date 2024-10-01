using FurrySharp;
using FurrySharp.States;
using FurrySharp.Test;

namespace FurrySharp.Test;

[TestFixture]
public class EntityManagerTests : FurryGameBaseTest
{
    [Test]
    public void IntegrationTest()
    {
        FurryGame.CreateAndSetState<SandboxState>();
        var sandboxState = (SandboxState)FurryGame.CurrentState;
        var result = sandboxState.EntityManager.Spawn("Player");
        Assert.True(result);
        result = sandboxState.EntityManager.Spawn("player");
        Assert.True(result);
        result = sandboxState.EntityManager.Spawn("your mom");
        Assert.False(result);
        var entity = sandboxState.EntityManager.GetEntity(0);
        Assert.NotNull(entity);
        result = sandboxState.EntityManager.RemoveEntity(entity);
        Assert.True(result);
        entity = sandboxState.EntityManager.GetEntity(0);
        Assert.Null(entity);
        Assert.Pass();
    }
}