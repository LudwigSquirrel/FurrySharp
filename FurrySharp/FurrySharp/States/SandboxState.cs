﻿using FurrySharp.Audio;
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

    public override void Create()
    {
        base.Create();
        EntityManager.Spawn<Player>();
        EntityManager.GetEntity(0).Position = new Vector2(TILE_SIZE, TILE_SIZE); //todo: implement a better way to get access to entities.
        Map = MapInfo.FromResources("debug");
        AudioManager.PlaySong(Map.Settings.Music);
    }

    public override void UpdateState()
    {
        EntityManager.UpdateEntities();
        EntityManager.DoCollisions(Map);
        EntityManager.PostUpdateEntities();
    }
    
    public override void DrawState()
    {
        EntityManager.DrawEntities();
        Map.DrawLayer(new Rectangle(0,0, GAME_WIDTH_IN_PIXELS, GAME_HEIGHT_IN_PIXELS), (int)MapLayer.BG, DrawOrder.BG, false);
    }
}