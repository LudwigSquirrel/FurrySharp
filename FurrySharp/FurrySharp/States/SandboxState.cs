using FurrySharp.Audio;
using FurrySharp.Drawing;
using FurrySharp.Entities;
using FurrySharp.Entities.Player;
using FurrySharp.Maps;
using Microsoft.Xna.Framework;

using static FurrySharp.Registry.GameConstants;

namespace FurrySharp.States;

public class SandboxState : State
{
    public EntityManager EntityManager = new();

    public MapInfo Map;

    public Player Player;

    public override void Create()
    {
        base.Create();
        EntityManager.Spawn(out Player);
        Player.Position = new Vector2(TILE_SIZE, TILE_SIZE);
        Map = MapInfo.FromResources("debug");
        EntityManager.Map = Map;
        AudioManager.PlaySong(Map.Settings.Music);
        GameTimes.TimeScale = 1f;
    }

    public override void UpdateState()
    {
        EntityManager.UpdateEntities();
        EntityManager.DoCollisions();
        // It seems that controlling the camera works best after this point. If done before update/collisions, then the
        // entity position the camera was given will be out of sync most frames. It's also futile to move the camera in
        // the DrawState() method, because the camera's transform was already given to the spritebatch - any changes in
        // position take place next frame, and also, Draw() and Update() are not in sync anyway, so there's no guarantee
        // you get the position at a consistent point in the game loop. I think it is safest to do camera movement
        // "post" update.
        SpriteDrawer.Camera.CenterOn((Player.Position.ToPoint() + Player.HitBox.Center).ToVector2());
        EntityManager.PostUpdateEntities();
        EntityManager.DoOnScreen();
    }
    
    public override void DrawState()
    {
        EntityManager.DrawEntities();
        Map.DrawLayer(SpriteDrawer.Camera.Bounds, (int)MapLayer.BG, DrawOrder.BG, false);
    }
}