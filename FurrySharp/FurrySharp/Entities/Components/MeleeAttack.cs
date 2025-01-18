using FurrySharp.Utilities;
using Microsoft.Xna.Framework;

namespace FurrySharp.Entities.Components;

public class MeleeAttack
{
    public Vector2 MeleeOrigin;
    public CatSpline MeleeTipTravelSpline;
    public float MeleeCollisionArc;
}