using FurrySharp.Entities.Base;
using Microsoft.Xna.Framework;

namespace FurrySharp.Entities.Components;

public class VelMover
{
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

    public void Move(Entity entity)
    {
        entity.Position += Velocity * GameTimes.DeltaTime;
    }

    private float GetLerpAmount(bool accelerating)
    {
        // todo: implement air acceleration/deacceleration.
        return accelerating ? AccelerateWalk : DeaccelerateWalk;
    }
}