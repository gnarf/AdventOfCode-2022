using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace AoC2022;

class Day12 : Puzzle
{

    public Dictionary<Point2D, int> heightMap = new();
    public Point2D start = Point2D.Zero;
    public Point2D goal = Point2D.Zero;
    public override void Parse(string filename)
    {
        base.Parse(filename);
        for (int y=0; y < lines.Length; y++)
        {
            for (int x=0; x < lines[y].Length; x++)
            {
                if (lines[y][x] == 'S') start = new Point2D(x, y);
                if (lines[y][x] == 'E') goal = new Point2D(x, y);
                heightMap.Add(new Point2D(x, y), lines[y][x] switch { 'S' => 0, 'E' => 25, char c => c - 'a' });
            }
        }
    }
    public override void Part1()
    {
        Dictionary<Point2D, int> visitCost = new() {};

        List<(Point2D point, int cost, int height)> toVisit = new List<(Point2D point, int cost, int height)>() { (start, 0, 0) };

        while(toVisit.Count > 0)
        {
            var (point, cost, height) = toVisit[0];
            toVisit.RemoveAt(0);
            if (visitCost.ContainsKey(point)) continue;
            visitCost.Add(point, cost);
            foreach (var direction in Point2D.FacingToPointVector.Values)
            {
                if (visitCost.ContainsKey(point + direction) || toVisit.Any(p => p.point == point + direction && p.cost <= cost + 1)) continue;
                if (heightMap.TryGetValue(point + direction, out var newheight))
                {
                    if (newheight <= height + 1)
                    {
                        toVisit.Add((point + direction, cost + 1, newheight));
                    }
                }
            }
            toVisit.Sort( (a, b) => a.cost - b.cost );
        }

        Console.WriteLine(visitCost[goal]);

    }

    public override void Part2()
    {
        Dictionary<Point2D, int> visitCost = new() {};

        List<(Point2D point, int cost, int height)> toVisit = new List<(Point2D point, int cost, int height)>() { (goal, 0, 25) };

        while(toVisit.Count > 0)
        {
            var (point, cost, height) = toVisit[0];
            toVisit.RemoveAt(0);
            if (visitCost.ContainsKey(point)) continue;
            visitCost.Add(point, cost);
            foreach (var direction in Point2D.FacingToPointVector.Values)
            {
                if (visitCost.ContainsKey(point + direction) || toVisit.Any(p => p.point == point + direction && p.cost <= cost + 1)) continue;
                if (heightMap.TryGetValue(point + direction, out var newheight))
                {
                    if (newheight + 1 >= height)
                    {
                        toVisit.Add((point + direction, cost + 1, newheight));
                    }
                }
            }
            toVisit.Sort( (a, b) => a.cost - b.cost );
        }

        Console.WriteLine(visitCost.Where(k => heightMap[k.Key] == 0).Min(s => s.Value));
    }
}