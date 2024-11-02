using System;
using FurrySharp.Entities.Base;
using Microsoft.Xna.Framework;

namespace FurrySharp.Entities.Components;

public class VelMover
{
    private Vector2 actualPosition;
    
    public float TargetSpeed; // Velocity will lerp to this value.
    public Vector2 TargetDirection; // Note: You can set this to whatever, but it WILL be normalized whether you like it or not.
    public Vector2 Velocity; // Modify this value to add an impulse.

    public float AccelerateWalk = 14f;
    public float AccelerateAir;
    public float DeaccelerateWalk = 28f;
    public float DeaccelerateAir;

    public void DoVel()
    {
        if (TargetDirection != Vector2.Zero)
        {
            TargetDirection.Normalize();
        }

        Vector2 targetVelocity = TargetDirection * TargetSpeed;
        bool accelerating = Vector2.Dot(targetVelocity, Velocity) > 0;
        float lerpAmount = GetLerpAmount(accelerating);

        Velocity = Vector2.Lerp(Velocity, targetVelocity, lerpAmount * GameTimes.DeltaTime);
    }
    
    // This causes the entity position to have decimals, and can make them "jitter" on the screen. I might use this for aesthetic reasons :3
    public void FloatyMove(Entity entity)
    {
        entity.Position += Velocity * GameTimes.DeltaTime;
    }

    // This moves the entity in whole increments. This is necessary for the camera to smoothly track an entity.
    public void WholeyMove(Entity entity)
    {
        actualPosition += Velocity * GameTimes.DeltaTime;

        while (MathF.Abs(actualPosition.X) > 1f)
        {
            var sign = MathF.Sign(actualPosition.X);
            entity.Position.X += sign;
            actualPosition.X -= sign;
        }
        
        while (MathF.Abs(actualPosition.Y) > 1f)
        {
            var sign = MathF.Sign(actualPosition.Y);
            entity.Position.Y += sign;
            actualPosition.Y -= sign;
        }
    }

    private float GetLerpAmount(bool accelerating)
    {
        // todo: implement air acceleration/deacceleration.
        return accelerating ? AccelerateWalk : DeaccelerateWalk;
    }
}