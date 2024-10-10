using FurrySharp.Entities;
using FurrySharp.Entities.Base;

namespace FurrySharp.Test;

public class CollisionGroupsTests
{
    [Collision()]
    private class Me : Entity
    {
    }
    
    [Collision()]
    private class MyClone : Me
    {
    }
    
    [Collision(typeof(Me))]
    private class YourMom : Entity
    {
        // I collide with your mom on a weekly basis.
    }
    
    [Collision(typeof(YourMom), typeof(MyClone))]
    private class YourDad : Entity
    {
    }

    [Test]
    public void CollisionGroupsTest()
    {
        var collisionGroups = new CollisionGroups();
        var entity = new Me();
        var entity2 = new MyClone();
        var entity3 = new YourMom();
        var entity4 = new YourDad();
        collisionGroups.Register(entity);
        collisionGroups.Register(entity2);
        collisionGroups.Register(entity3);
        collisionGroups.Register(entity4);
        var group = collisionGroups.Get(typeof(Me));
        var group2 = collisionGroups.Get(typeof(YourMom));
        
        Assert.That(group.Colliders.Count, Is.EqualTo(1)); // The one Mom.
        Assert.That(group.Targets.Count, Is.EqualTo(2)); // Me and my clone.
        
        Assert.That(group2.Colliders.Count, Is.EqualTo(1)); // The one Dad.
        Assert.That(group2.Targets.Count, Is.EqualTo(1)); // The one Mom.

        // Unregister the clone and your dad, so I can have your Mom all to myself.
        Assert.That(collisionGroups.Unregister(entity2), Is.True);
        Assert.That(collisionGroups.Unregister(entity4), Is.True);
        Assert.That(collisionGroups.Unregister(new Entity()), Is.False);
        
        Assert.That(group.Colliders.Count, Is.EqualTo(1)); // The one Mom.
        Assert.That(group.Targets.Count, Is.EqualTo(1)); // Just me.
        
        Assert.That(group2.Colliders.Count, Is.EqualTo(0)); // No one. Sad.
        Assert.That(group2.Targets.Count, Is.EqualTo(1)); // The one Mom.
    }
}