#nullable enable
using FurrySharp.Entities;
using FurrySharp.Entities.Base;
using FurrySharp.Maps;
using Microsoft.Xna.Framework;

namespace FurrySharp.Utilities;

public struct RayCastResult
{
    public DDAResult? DDAResult;
    public EntityCastResult? EntityCastResult;
}