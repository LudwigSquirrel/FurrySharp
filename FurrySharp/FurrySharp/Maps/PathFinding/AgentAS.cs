using System;
using Microsoft.Xna.Framework;

namespace FurrySharp.Maps.PathFinding;

public class AgentAS
{
    public Func<Point,Point,float> Heuristic = Manhattan;
    public int SearchLimit { get; set; }
    public bool AcceptPathToBestHeuristic { get; set; } // if a path is not found, a path to the point with the best heuristic is returned.
    public bool UseDiagonals { get; set; }

    public static float Manhattan(Point start, Point end)
    {
        return Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y);
    }
}