using System;
using System.Collections.Generic;
using FurrySharp.Drawing;
using FurrySharp.Entities.Base;
using FurrySharp.Entities.Components;
using FurrySharp.Entities.Enemy;
using FurrySharp.Entities.Player;
using FurrySharp.Input;
using FurrySharp.Maps.PathFinding;
using FurrySharp.Resources;
using FurrySharp.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FurrySharp.Entities.Hostiles;

[Collision(typeof(PlayerEntity), typeof(Nudwig), MapCollider = true, RayCastable = true)]
[Enemy]
public class Nudwig : Entity
{
    private AgentAS AgentAS = new AgentAS() { AcceptPathToBestHeuristic = true, SearchLimit = 1000 };
    private TickyTimer RepathTimer = new() { Loop = true, SecondInterval = 1f };
    private List<Vector2> Path = new();
    private CatSpline Spline = new CatSpline();
    private VelMover Mover = new VelMover();
    private PathFollower Follower = new PathFollower();
    
    private Texture2D NudwigTexture;

    public Nudwig()
    {
        Follower.Path = Spline;
        HitBox = new Rectangle(11, 16, 11, 10);
        HitRadius = 16f;
        
        NudwigTexture = ResourceManager.GetTexture("ludwig_player");
        BoundingBox = EntityUtilities.BoundingBoxFrom(NudwigTexture);
    }

    public override void Update()
    {
        if (RepathTimer.DoTick())
            // if (GameInput.JustPressedKey(Keys.O))
        {
            Vector2 end = Manager.Player?.EntityCenter ?? Vector2.Zero; // todo: get random point on map.
            Path = Map.GridAS.GetPathWithMapLoc(AgentAS, EntityCenter, end) ?? new List<Vector2>() { Vector2.Zero };
            Spline.ControlPoints = Path;
            if (Spline.NotHasMinimumNumberOfControlPoints() == false)
            {
                Spline.UpdateLengthAndLookupTable();
                Follower.PlaceCarrotAtStart();
            }
        }

        Mover.TargetDirection = Vector2.Zero;
        Mover.TargetSpeed = 64f;
        if (Spline.NotHasMinimumNumberOfControlPoints() || Follower.CarrotAtEnd())
        {
            Mover.TargetDirection = Manager.Player!.EntityCenter - EntityCenter;
        }
        else
        {
            Follower.OrientMoverToCarrot(this, Mover);
            Follower.MoveCarrotAlongPath(this, 32f);
        }

        Mover.DoVel();
        Mover.WholeyMove(this);
    }

    public override void Draw()
    {
        base.Draw();
        SpriteDrawer.DrawSprite(NudwigTexture, new Rectangle(Position.ToPoint(), NudwigTexture.Bounds.Size), z: EntityUtilities.GetEntityZ(this));
    }

    public override void Collided(Entity other)
    {
        if (other is PlayerEntity)
        {
            other.Position = new Vector2(32, 32); // todo: respawn player.
        }
        else
        {
            EntityUtilities.SeparateEntityFromEntity(this, other);
        }
    }
}