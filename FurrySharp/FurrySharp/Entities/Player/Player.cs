using FurrySharp.Drawing;
using FurrySharp.Entities.Base;
using FurrySharp.Entities.Components;
using FurrySharp.Input;
using FurrySharp.Logging;
using FurrySharp.Maps;
using FurrySharp.Resources;
using FurrySharp.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FurrySharp.Entities.Player;

[Collision(MapCollider = true)]
public class Player : Entity
{
    Spritesheet PlayerSpriteSheet;
    public VelMover Mover;

    public Texture2D SwordTexture;
    public Trail SwordSwipeTrail;

    public Player()
    {
        PlayerSpriteSheet = new Spritesheet(ResourceManager.GetTexture("ludwig_player"), 32, 32);
        
        SwordSwipeTrail = new Trail();
        SwordSwipeTrail.AddDefaultUnits(10);
        SwordSwipeTrail.Spritesheet = PlayerSpriteSheet;

        SwordTexture = ResourceManager.GetTexture("sword");
        
        BoundingBox = EntityUtilities.BoundingBoxFromSpritesheet(PlayerSpriteSheet);
        HitBox = new Rectangle(11, 16, 11, 10);
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
        var antiKiss = 6;
        var walkAroundAssist = 0.95f;

        Mover.TargetSpeed = walkSpeed;
        Mover.TargetDirection = Vector2.Zero;

        Rectangle antiKisser = new Rectangle((int)Position.X + HitBox.X, (int)Position.Y + HitBox.Y, HitBox.Width, HitBox.Height);
        antiKisser.Inflate(antiKiss, antiKiss); // owo inflation

        // UP
        if (GameInput.IsFunctionPressed(KeyFunctions.Up))
        {
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
        if (GameInput.JustPressedFunction(KeyFunctions.Attack1))
        {
            
        }
    }

    public override void Draw()
    {
        base.Draw();
        SpriteDrawer.DrawSprite(PlayerSpriteSheet.Tex, Position, z: EntityUtilities.GetEntityZ(this));
    }
}