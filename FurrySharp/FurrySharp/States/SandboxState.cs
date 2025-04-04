﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FurrySharp.Audio;
using FurrySharp.Drawing;
using FurrySharp.Entities;
using FurrySharp.Entities.Components;
using FurrySharp.Entities.Hostiles;
using FurrySharp.Entities.Player;
using FurrySharp.Input;
using FurrySharp.Maps;
using FurrySharp.Maps.PathFinding;
using FurrySharp.Registry;
using FurrySharp.Resources;
using FurrySharp.UI;
using ImGuiNET;
using FurrySharp.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using static FurrySharp.Registry.GameConstants;

namespace FurrySharp.States;

public class SandboxState : State
{
    public EntityManager EntityManager = new();

    public MapInfo Map;

    public PlayerEntity PlayerEntity;

    public FingyCursor FingyCursor = new();

    public Trail Trail = new();

    public GridAS GridAS;

    public Vector2 FingyPos;

    public List<Point> Path = new();

    public TickyTimer SpawnTimer = new() { Loop = true, SecondInterval = 1f };
    // public Spline BezierCurve = new();

    public override void Create()
    {
        base.Create();
        EntityManager.Spawn(out PlayerEntity);
        PlayerEntity.Position = new Vector2(TILE_SIZE, TILE_SIZE);
        EntityManager.Player = PlayerEntity;
        // EntityManager.Spawn(out Player friendo);
        // friendo.Position = new Vector2(TILE_SIZE + TILE_SIZE, TILE_SIZE);
        Map = MapInfo.FromResources("maze1");
        EntityManager.Map = Map;
        AudioManager.PlaySong(Map.Settings.Music);
        GameTimes.TimeScale = 1f;

        Spritesheet spritesheet = new Spritesheet(ResourceManager.GetTexture("ludwig_player"), 32, 32);
        Trail.AddDefaultUnits(20);
        Trail.Spritesheet = spritesheet;

        // BezierCurve.Segments.Add(new BezierCurve()
        // {
        //     A = new Vector2(TILE_SIZE * 5, TILE_SIZE * 5),
        //     B = new Vector2(TILE_SIZE * 5, 0),
        //     C = new Vector2(TILE_SIZE * 10, 0),
        //     D = new Vector2(TILE_SIZE * 10, TILE_SIZE * 5),
        // });
        // BezierCurve.UpdateLengthAndLookupTable();
    }

    public override void UpdateState()
    {
        //Vector2[] circle = MathUtilities.PlotCircle(new Vector2(TILE_SIZE * 5, TILE_SIZE * 5), 64, Trail.Units.Count, GameTimes.TotalTime);
        // Spline.Segments[0].A = Player.Position;
        // Spline.UpdateLengthAndLookupTable();
        // Vector2[] points = Spline.GetEvenlySpacedPoints(Trail.Units.Count);
        // for (var i = 0; i < Trail.Units.Count; i++)
        // {
        //     Trail.Units[i].Position = points[i];
        //     Trail.Units[i].Scale = MathF.Abs(MathF.Sin(GameTimes.TotalTime * 2 + i * 0.1f));
        //     Trail.Units[i].Rotation = GameTimes.TotalTime * 0.1f * i;
        // }
        
        EntityManager.UpdateEntities();
        EntityManager.DoCollisions();
        // It seems that controlling the camera works best after this point. If done before update/collisions, then the
        // entity position the camera was given will be out of sync most frames. It's also futile to move the camera in
        // the DrawState() method, because the camera's transform was already given to the spritebatch - any changes in
        // position take place next frame, and also, Draw() and Update() are not in sync anyway, so there's no guarantee
        // you get the position at a consistent point in the game loop. I think it is safest to do camera movement
        // "post" update.
        SpriteDrawer.Camera.CenterOn((PlayerEntity.Position.ToPoint() + PlayerEntity.HitBox.Center).ToVector2());
        EntityManager.PostUpdateEntities();
        EntityManager.DoOnScreen();
        
        FingyPos = SpriteDrawer.Camera.ScreenToWorld(GameInput.PointerScreenPosition.ToVector2());
        if (GameInput.IsKeyPressed(Keys.P))
        {
            EntityManager.Spawn(out Nudwig legume);
            legume.Position = FingyPos;
        }

        if (SpawnTimer.DoTick())
        {
            EntityManager.Spawn(out Nudwig legume);
            legume.Position = new Vector2(TILE_SIZE * GlobalState.RNG.Next(Map.Width), GlobalState.RNG.Next(Map.Height));
        }
    }
    
    public override void DrawState()
    {
        EntityManager.DrawEntities();
        Trail.DrawTrail(Map);
        Map.DrawLayer(SpriteDrawer.Camera.Bounds, (int)MapLayer.BG, DrawOrder.BG, false);

        var start = PlayerEntity.EntityCenter;
        var end = FingyPos;
        DDAResult result = PlayerEntity.Map.DDA(start, end);
        // SpriteDrawer.DrawDebugLine(start, result.End, result.TileFound ? Color.Red : Color.White);
        //foreach (var point in result.Visited)
        //{
            //SpriteDrawer.DrawSprite(SpriteDrawer.SolidTex, new Rectangle(point.X * TILE_SIZE, point.Y * TILE_SIZE, TILE_SIZE, TILE_SIZE), color: Color.Yellow);
        //}
        SpriteDrawer.DrawLines(Path.Select(point => point.ToVector2() * TILE_SIZE + new Vector2(TILE_SIZE/2,TILE_SIZE/2)).ToArray(), Color.DarkRed, closed: false);
    }

    public override void DrawUI()
    {
        FingyCursor.DrawFingy();
    }

    public override void DoIMGUI()
    {
        ImGui.Begin("Sandbox State");
        if (ImGui.Button("Spawn a friend"))
        {
            EntityManager.Spawn(out PlayerEntity friend);
            friend.Position = PlayerEntity.Position + Vector2.One;
        }
        ImGui.End();
    }
}