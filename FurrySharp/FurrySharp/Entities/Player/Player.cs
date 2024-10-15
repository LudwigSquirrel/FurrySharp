using FurrySharp.Drawing;
using FurrySharp.Entities.Base;
using FurrySharp.Input;
using FurrySharp.Resources;
using FurrySharp.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FurrySharp.Entities.Player;

[Collision(MapCollider = true)]
public class Player : Entity
{
    public Texture2D PlayerSprite;

    public Player()
    {
        PlayerSprite = ResourceManager.GetTexture("ludwig_player");
        BoundingBox = EntityUtilities.BoundingBoxFromTexture(PlayerSprite);
        HitBox = new Rectangle(11, 16,11,12);
    }

    public override void Update()
    {
        if (KeyInput.IsFunctionPressed(KeyFunctions.Up))
        {
            MoveUp();
        }
        if (KeyInput.IsFunctionPressed(KeyFunctions.Down))
        {
            MoveDown();
        }
        if (KeyInput.IsFunctionPressed(KeyFunctions.Left))
        {
            MoveLeft();
        }
        if (KeyInput.IsFunctionPressed(KeyFunctions.Right))
        {
            MoveRight();
        }
    }

    public override void Draw()
    {
        base.Draw();
        SpriteDrawer.DrawSprite(PlayerSprite, Position);
    }

    private void MoveUp()
    {
        Position.Y -= 1; // Adjust the value as needed
    }

    private void MoveDown()
    {
        Position.Y += 1; // Adjust the value as needed
    }

    private void MoveLeft()
    {
        Position.X -= 1; // Adjust the value as needed
    }

    private void MoveRight()
    {
        Position.X += 1; // Adjust the value as needed
    }
}