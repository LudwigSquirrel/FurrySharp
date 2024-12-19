#nullable enable
using FurrySharp.Entities.Base;
using Microsoft.Xna.Framework;

namespace FurrySharp.Utilities;

public struct RayCastResult
{
    public Vector2 HitPosition;
    public Vector2 HitNormal;

    public bool WasTileHit;
    public int TileLayer;
    public Point TilePoint;
    public int TileIndex;

    public Entity? HitEntity;
}