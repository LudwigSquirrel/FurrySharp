#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using FurrySharp.Entities.Base;
using FurrySharp.Maps.Tiles;
using Microsoft.Xna.Framework;
using static FurrySharp.Registry.GameConstants;

namespace FurrySharp.Maps.PathFinding;

public class GridAS
{
    private readonly MapInfo MapInfo;

    private readonly PriorityQueue<Point, float> OpenQueue = new();
    private readonly HashSet<Point> OpenSet = new();

    public GridAS(MapInfo map)
    {
        MapInfo = map;
    }

    public List<Vector2>? GetPathWithMapLoc(AgentAS agent, Vector2 start, Vector2 end)
    {
        List<Point>? points = GetPath(agent, MapInfo.ToMapLoc(start), MapInfo.ToMapLoc(end));
        return points?.Select(point => new Vector2(point.X * TILE_SIZE + TILE_SIZE_HALF, point.Y * TILE_SIZE + TILE_SIZE_HALF)).ToList();
    }

    public List<Point>? GetPath(AgentAS agent, Point start, Point end)
    {
        OpenClear();
        OpenAdd(start, 0f);

        var searches = 0;
        var bestHeuristicValue = float.MaxValue;
        var bestHeuristicPoint = start;
        var cameFrom = new Dictionary<Point, Point>(MapInfo.Width * MapInfo.Height);
        var gScore = new Dictionary<Point, float>();
        var fScore = new Dictionary<Point, float>();
        gScore[start] = 0f;
        fScore[start] = 0f;

        while (OpenQueue.Count > 0)
        {
            Point current = OpenDequeue();
            if (current == end)
            {
                return ReconstructPath(current);
            }

            if (searches == agent.SearchLimit)
            {
                return agent.AcceptPathToBestHeuristic ? ReconstructPath(bestHeuristicPoint) : null;
            }

            foreach (var neighbor in NeighborsOf(current))
            {
                var g = gScore.GetValueOrDefault(neighbor.point, float.MaxValue);
                var tentativeG = gScore.GetValueOrDefault(current, float.MaxValue) + neighbor.distance;
                if (tentativeG < g)
                {
                    cameFrom[neighbor.point] = current;
                    gScore[neighbor.point] = tentativeG;
                    
                    var agentHeuristic = agent.Heuristic(neighbor.point, end);
                    if (agentHeuristic < bestHeuristicValue)
                    {
                        bestHeuristicValue = agentHeuristic;
                        bestHeuristicPoint = neighbor.point;
                    }
                    fScore[neighbor.point] = tentativeG + agentHeuristic;
                    if (!OpenContains(neighbor.point))
                    {
                        OpenAdd(neighbor.point, fScore[neighbor.point]);
                    }
                }
            }

            searches += 1;
        }

        return null;

        List<Point> ReconstructPath(Point current)
        {
            var list = new List<Point>();
            list.Add(current);
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                list.Insert(0, current);
            }

            return list;
        }

        IEnumerable<(Point point, float distance)> NeighborsOf(Point point)
        {
            bool tm = MapInfo.GetCollisionData(point.X - 0, point.Y - 1) == Touching.NONE; // todo: allow Touching.LEFT and Touching.BOTTOM
            bool ml = MapInfo.GetCollisionData(point.X - 1, point.Y - 0) == Touching.NONE;
            bool mr = MapInfo.GetCollisionData(point.X + 1, point.Y - 0) == Touching.NONE;
            bool bm = MapInfo.GetCollisionData(point.X - 0, point.Y + 1) == Touching.NONE;

            if (tm) yield return (new Point(point.X - 0, point.Y - 1), 1f);
            if (ml) yield return (new Point(point.X - 1, point.Y - 0), 1f);
            if (mr) yield return (new Point(point.X + 1, point.Y - 0), 1f);
            if (bm) yield return (new Point(point.X - 0, point.Y + 1), 1f);

            if (agent.UseDiagonals)
            {
                bool tl = MapInfo.GetCollisionData(point.X - 1, point.Y - 1) == Touching.NONE;
                bool tr = MapInfo.GetCollisionData(point.X + 1, point.Y - 1) == Touching.NONE;
                bool bl = MapInfo.GetCollisionData(point.X - 1, point.Y + 1) == Touching.NONE;
                bool br = MapInfo.GetCollisionData(point.X + 1, point.Y + 1) == Touching.NONE;

                if (bl && bm && ml) yield return (new Point(point.X - 1, point.Y + 1), 1.41f);
                if (br && bm && mr) yield return (new Point(point.X + 1, point.Y + 1), 1.41f);
                if (tl && tm && ml) yield return (new Point(point.X - 1, point.Y - 1), 1.41f);
                if (tr && tm && mr) yield return (new Point(point.X + 1, point.Y - 1), 1.41f);
            }
        }
    }

    private void OpenClear()
    {
        OpenQueue.Clear();
        OpenSet.Clear();
    }

    private void OpenAdd(Point point, float priority)
    {
        OpenQueue.Enqueue(point, priority);
        OpenSet.Add(point);
    }

    private Point OpenDequeue()
    {
        var openDequeue = OpenQueue.Dequeue();
        OpenSet.Remove(openDequeue);
        return openDequeue;
    }

    private bool OpenContains(Point point)
    {
        return OpenSet.Contains(point);
    }
}