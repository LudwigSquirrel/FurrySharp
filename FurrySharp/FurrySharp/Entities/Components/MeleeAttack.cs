using FurrySharp.Entities.Base;
using FurrySharp.Resources;
using FurrySharp.Utilities;
using Microsoft.Xna.Framework;

namespace FurrySharp.Entities.Components;

public class MeleeAttack
{
    // TODO: I'm actually not gonna implement any of this lol.
    public Facing Facing;
    public Vector2 MeleeOrigin;
    public float Duration;
    public string MeleeTipTravelSplineName;
    public CatSpline MeleeTipTravelSpline => ResourceManager.GetSpline(MeleeTipTravelSplineName); // The point from this spline will be translated to be oriented based on player.
}