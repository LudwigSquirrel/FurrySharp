using FurrySharp.Drawing;
using FurrySharp.Entities.Base;
using FurrySharp.Entities.Components;
using FurrySharp.Input;
using FurrySharp.Logging;
using FurrySharp.Resources;
using FurrySharp.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FurrySharp.Entities.Player;

[Collision(MapCollider = true)]
public class Player : Entity
{
    public Texture2D PlayerSprite;
    public VelMover Mover;

    public Player()
    {
        PlayerSprite = ResourceManager.GetTexture("ludwig_player");
        BoundingBox = EntityUtilities.BoundingBoxFromTexture(PlayerSprite);
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
        var antiBoop = 6;
        var walkAroundAssist = 0.95f;
        
        Mover.TargetSpeed = walkSpeed;
        Mover.TargetDirection = Vector2.Zero;

        Rectangle antiBooper = new Rectangle((int)Position.X + HitBox.X, (int)Position.Y + HitBox.Y, HitBox.Width, HitBox.Height);
        antiBooper.Inflate(antiBoop, antiBoop); // owo inflation

        // UP
        if (GameInput.IsFunctionPressed(KeyFunctions.Up))
        {
            // Don't boop walls!
            if ((WasTouching & Touching.UP) == 0)
            {
                Mover.TargetDirection.Y += -1f;
            }
            else if (GameInput.IsFunctionPressed(KeyFunctions.Left) == false && GameInput.IsFunctionPressed(KeyFunctions.Right) == false)
            {
                // Walk around them!
                Touching tl = Map.GetCollisionData(antiBooper.Left, antiBooper.Top);
                Touching tr = Map.GetCollisionData(antiBooper.Right, antiBooper.Top);
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
                Touching bl = Map.GetCollisionData(antiBooper.Left, antiBooper.Bottom);
                Touching br = Map.GetCollisionData(antiBooper.Right, antiBooper.Bottom);
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
                Touching tl = Map.GetCollisionData(antiBooper.Left, antiBooper.Top);
                Touching bl = Map.GetCollisionData(antiBooper.Left, antiBooper.Bottom);
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
                Touching tr = Map.GetCollisionData(antiBooper.Right, antiBooper.Top);
                Touching br = Map.GetCollisionData(antiBooper.Right, antiBooper.Bottom);
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
            DebugLogger.AddDebug("Attack 1 pressed");
        }
    }
    public override void Draw()
    {
        base.Draw();
        SpriteDrawer.DrawSprite(PlayerSprite, Position, z: DrawingUtilities.GetDrawingZ(DrawOrder.Player));
    }
}