﻿using System;
using FurrySharp.Animation;
using FurrySharp.Drawing;
using FurrySharp.Entities.Base;
using FurrySharp.Entities.Components;
using FurrySharp.Entities.Hostiles;
using FurrySharp.Input;
using FurrySharp.Logging;
using FurrySharp.Maps;
using FurrySharp.Resources;
using FurrySharp.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FurrySharp.Entities.Player;

[Collision(MapCollider = true, RayCastable = true)]
public class PlayerEntity : Entity
{
    Spritesheet PlayerSpriteSheet;
    public VelMover Mover;

    public Texture2D SwordTexture;

    public float SwordSwipiness;
    public float SwordSwipinessT;
    public float SwordLength = 64f;
    public float SwipeSpeed = 4f;
    public float SwipeArc = 90f;
    public Vector2 SwordTip;

    public PlayerEntity()
    {
        PlayerSpriteSheet = new Spritesheet(ResourceManager.GetTexture("ludwig_handsome"), 64, 64);

        SwordTexture = ResourceManager.GetTexture("sword");

        BoundingBox = EntityUtilities.BoundingBoxFrom(PlayerSpriteSheet);
        HitBox = new Rectangle(11, 16, 11, 10);
        HitRadius = 16f;
        Mover = new VelMover();
    }

    public override void Update()
    {
        Frolic();
        SwordSwipe();
        Mover.DoVel();
        Mover.WholeyMove(this);
    }

    // Ground movement logic.
    public void Frolic()
    {
        var walkSpeed = 96f;
        var walkAndSwipeSpeed = 48f;
        var antiKiss = 6;
        var walkAroundAssist = 0.95f;

        Mover.TargetSpeed = MathHelper.Lerp(walkSpeed, walkAndSwipeSpeed, SwordSwipiness);
        Mover.TargetDirection = Vector2.Zero;

        Rectangle antiKisser = new Rectangle((int)Position.X + HitBox.X, (int)Position.Y + HitBox.Y, HitBox.Width, HitBox.Height);
        antiKisser.Inflate(antiKiss, antiKiss); // owo inflation

        bool doTurn = SwordSwipiness == 0;

        // UP
        if (GameInput.IsFunctionPressed(KeyFunctions.Up))
        {
            if (doTurn)
            {
                Facing = Facing.N;
            }

            // Don't kiss walls...
            if ((WasTouching & Touching.UP) == 0)
            {
                Mover.TargetDirection.Y += -1f;
            }
            else if (GameInput.IsFunctionPressed(KeyFunctions.Left) == false && GameInput.IsFunctionPressed(KeyFunctions.Right) == false)
            {
                // ...walk around them!
                Touching tl = Map.GetCollisionData(antiKisser.Left, antiKisser.Top);
                Touching tr = Map.GetCollisionData(antiKisser.Right, antiKisser.Top);
                if ((tl & Touching.DOWN) == 0)
                {
                    Mover.TargetDirection.X += -walkAroundAssist;
                }
                else if ((tr & Touching.DOWN) == 0)
                {
                    Mover.TargetDirection.X += walkAroundAssist;
                }
            }
        }

        // DOWN
        if (GameInput.IsFunctionPressed(KeyFunctions.Down))
        {
            if (doTurn)
            {
                Facing = Facing.S;
            }

            if ((WasTouching & Touching.DOWN) == 0)
            {
                Mover.TargetDirection.Y += 1f;
            }
            else if (GameInput.IsFunctionPressed(KeyFunctions.Left) == false && GameInput.IsFunctionPressed(KeyFunctions.Right) == false)
            {
                Touching bl = Map.GetCollisionData(antiKisser.Left, antiKisser.Bottom);
                Touching br = Map.GetCollisionData(antiKisser.Right, antiKisser.Bottom);
                if ((bl & Touching.UP) == 0)
                {
                    Mover.TargetDirection.X += -walkAroundAssist;
                }
                else if ((br & Touching.UP) == 0)
                {
                    Mover.TargetDirection.X += walkAroundAssist;
                }
            }
        }

        // LEFT
        if (GameInput.IsFunctionPressed(KeyFunctions.Left))
        {
            if (doTurn)
            {
                Facing = Facing.W;
            }

            if ((WasTouching & Touching.LEFT) == 0)
            {
                Mover.TargetDirection.X += -1f;
            }
            else if (GameInput.IsFunctionPressed(KeyFunctions.Up) == false && GameInput.IsFunctionPressed(KeyFunctions.Down) == false)
            {
                Touching tl = Map.GetCollisionData(antiKisser.Left, antiKisser.Top);
                Touching bl = Map.GetCollisionData(antiKisser.Left, antiKisser.Bottom);
                if ((tl & Touching.RIGHT) == 0)
                {
                    Mover.TargetDirection.Y += -walkAroundAssist;
                }
                else if ((bl & Touching.RIGHT) == 0)
                {
                    Mover.TargetDirection.Y += walkAroundAssist;
                }
            }
        }

        // RIGHT
        if (GameInput.IsFunctionPressed(KeyFunctions.Right))
        {
            if (doTurn)
            {
                Facing = Facing.E;
            }

            if ((WasTouching & Touching.RIGHT) == 0)
            {
                Mover.TargetDirection.X += 1f;
            }
            else if (GameInput.IsFunctionPressed(KeyFunctions.Up) == false && GameInput.IsFunctionPressed(KeyFunctions.Down) == false)
            {
                Touching tr = Map.GetCollisionData(antiKisser.Right, antiKisser.Top);
                Touching br = Map.GetCollisionData(antiKisser.Right, antiKisser.Bottom);
                if ((tr & Touching.LEFT) == 0)
                {
                    Mover.TargetDirection.Y += -walkAroundAssist;
                }
                else if ((br & Touching.LEFT) == 0)
                {
                    Mover.TargetDirection.Y += walkAroundAssist;
                }
            }
        }
    }

    public void SwordSwipe()
    {
        bool doCast = true;
        if (GameInput.IsFunctionPressed(KeyFunctions.Attack1))
        {
            MathUtilities.MoveTo(ref SwordSwipiness, 1f, 8f);
            SwordSwipinessT = MathHelper.Lerp(SwordSwipinessT, AnimUtility.EaseInSine(SwordSwipiness), 0.5f);
        }
        else if (SwordSwipinessT > 0.01f)
        {
            MathUtilities.MoveTo(ref SwordSwipiness, 0f, 16f);
            SwordSwipinessT = MathHelper.Lerp(SwordSwipinessT, AnimUtility.EaseOutSine(SwordSwipiness), 0.5f);
        }
        else
        {
            SwordSwipiness = 0f;
            doCast = false;
        }

        if (doCast)
        {
            float offset = Facing switch
            {
                Facing.E => 0f,
                Facing.N => -270f,
                Facing.W => -180f,
                Facing.S => -90f,
                _ => throw new ArgumentOutOfRangeException()
            };
            float rad1 = MathHelper.ToRadians((SwipeArc / 2) - offset);
            float rad2 = MathHelper.ToRadians((SwipeArc / -2) - offset);
            float animT = MathUtilities.TriangleWave(GameTimes.TotalTime * SwipeSpeed, 1f);
            float thetaT = MathHelper.Lerp(rad1, rad2, AnimUtility.EaseInOutCirc(animT));
            SwordTip = new Vector2(
                x: SwordLength * SwordSwipinessT * MathF.Cos(thetaT),
                y: SwordLength * SwordSwipinessT * MathF.Sin(thetaT)
            );

            RayCastResult result;
            int casts = 0;
            do
            {
                result = Manager.RayCast<Nudwig>(EntityCenter, EntityCenter + SwordTip);
                if (result.EntityCastResult.WasHit)
                {
                    // todo: damage enemy.
                    Manager.RemoveEntity(result.EntityCastResult.Entity);
                }
                casts++;
            } while(result.EntityCastResult.WasHit && casts < 3);
        }
    }

    public override void Draw()
    {
        base.Draw();
        var frame = Facing switch
        {
            Facing.S => 0,
            Facing.E => 3,
            Facing.W => 1,
            Facing.N => 2,
            _ => 0
        };
        var rect = PlayerSpriteSheet.GetRect(frame);
        SpriteDrawer.DrawSprite(PlayerSpriteSheet.Tex, Position - new Vector2(rect.Width,rect.Height) / 4, z: EntityUtilities.GetEntityZ(this), sRect:rect);
        if (SwordSwipinessT > 0)
        {
            SpriteDrawer.DrawLine(EntityCenter, EntityCenter + SwordTip, Color.Blue);
        }
    }

    public Vector2 GetVectorToPointer()
    {
        return SpriteDrawer.Camera.ScreenToWorld(GameInput.PointerScreenPosition.ToVector2()) - EntityCenter;
    }
}