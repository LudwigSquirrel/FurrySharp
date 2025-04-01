using FurrySharp.Entities;
using FurrySharp.Entities.Base;
using Microsoft.Xna.Framework;

namespace FurrySharp.Test;

public class CollisionGroupsTests
{
    [Collision()]
    private class Me : Entity
    {
        public Me()
        {
            HitRadius = 1;
        }
    }
    
    [Collision()]
    private class MyClone : Me
    {
        public MyClone()
        {
            HitRadius = 1;
        }
    }
    
    [Collision(typeof(Me))]
    [Collision(RayCastable = true)]
    private class YourMom : Entity
    {
        public YourMom()
        {
            HitRadius = 3;
        }
    }
    
    [Collision(typeof(YourMom), typeof(MyClone))]
    [Collision(RayCastable = true)]
    private class YourDad : Entity
    {
        public YourDad()
        {
            HitRadius = 2;
        }
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

    [Test]
    public void RayCastTest()
    {
        var collisionGroup = new CollisionGroups();
        var dad1 = new YourDad() { InstanceId = 0, Position = Vector2.Zero };
        var me = new Me() { Position = new Vector2(5, 5) }; // Not raycastable. Should not get hit.
        var dad2 = new YourDad() { InstanceId = 1, Position = new Vector2(10, 0) };
        
        collisionGroup.Register(dad1);
        collisionGroup.Register(dad2);

        // Raycast from 0,0 to 10,0. Should hit.
        EntityCastResult result = collisionGroup.RaycastForEntity<YourDad>(Vector2.Zero, new Vector2(10, 0), excluding: dad1);
        Assert.That(result.WasHit, Is.True);
        Assert.That(result.Entity?.InstanceId, Is.EqualTo(1));
        Assert.That(result.CircleLineIntersection.Intersects, Is.True);
        Assert.That(result.CircleLineIntersection.Intersections, Is.EqualTo(1));
        Assert.That(result.CircleLineIntersection.IntersectionPoints[0], Is.EqualTo(new Vector2(10 - dad2.HitRadius,0)));
        
        // Raycast from 0,0 to 10,0. Should hit.
        result = collisionGroup.RaycastForEntity<YourDad>(Vector2.Zero, new Vector2(10, 0), excluding: dad2);
        Assert.That(result.WasHit, Is.True);
        Assert.That(result.Entity?.InstanceId, Is.EqualTo(0));
        Assert.That(result.CircleLineIntersection.Intersects, Is.True);
        Assert.That(result.CircleLineIntersection.Intersections, Is.EqualTo(1));
        Assert.That(result.CircleLineIntersection.IntersectionPoints[0], Is.EqualTo(new Vector2(dad1.HitRadius,0)));
        
        // Raycast from 0,10 to 10,10. Should not hit.
        result = collisionGroup.RaycastForEntity<YourDad>(new Vector2(0, 10), new Vector2(10, 10));
        Assert.That(result.WasHit, Is.False);
        Assert.That(result.Entity, Is.Null);
        
        // Raycast from 0,10 to 0,5. Should not hit.
        result = collisionGroup.RaycastForEntity<YourDad>(new Vector2(0, 10), new Vector2(0, 5));
        Assert.That(result.WasHit, Is.False);
        Assert.That(result.Entity, Is.Null);
        
        // Raycast for non-raycastable entity. Should not hit.
        result = collisionGroup.RaycastForEntity<Me>(Vector2.Zero, new Vector2(10, 0));
        Assert.That(result.WasHit, Is.False);
        Assert.That(result.Entity, Is.Null);
    }
}