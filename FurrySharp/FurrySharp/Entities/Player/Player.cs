using FurrySharp.Drawing;
using FurrySharp.Entities.Base;
using FurrySharp.Input;
using FurrySharp.Resources;
using Microsoft.Xna.Framework.Graphics;

namespace FurrySharp.Entities.Player;

public class Player : Entity
{
    public Texture2D PlayerSprite;

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
        PlayerSprite ??= ResourceManager.GetTexture("ludwig_player");
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