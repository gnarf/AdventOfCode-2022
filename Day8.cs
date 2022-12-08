using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;

namespace AoC2022;

class Day8 : Puzzle
{
    Dictionary<Point2D, int> treeMap = new();

    public void ParseTrees()
    {
        for (int y=0; y<lines.Length; y++)
        {
            for (int x=0; x<lines[y].Length; x++)
            {
                treeMap.Add(new Point2D(x, y), lines[y][x] - '0');
            }
        }
    }

    public override void Part1()
    {
        ParseTrees();
        HashSet<Point2D> seenTrees = new();
        var swap = lines.Length - 1;
        for (int x=0; x<lines.Length; x++)
        {
            foreach (var facing in Point2D.FacingToPointVector.Keys)
            {
                int max = -1;
                var step = Point2D.FacingToPointVector[facing];
                var start = new Point2D
                (
                    x: step.x switch { 0 => x, 1 => 0, -1 => swap, _ => 0},
                    y: step.y switch { 0 => x, 1 => 0, -1 => swap, _ => 0}
                );
                for (int y=0; y<lines.Length; y++)
                {
                    var treeAt = start + (step * y);
                    var value = treeMap[treeAt];
                    if (value > max)
                    {
                        seenTrees.Add(treeAt);
                        max = value;
                    }
                }
            }
        }
        Console.WriteLine(seenTrees.Count());
    }

    public override void Part2()
    {
        var count =  lines.Length;
        var r = treeMap.Select( kv => {
            var treeAt = kv.Key;
            return Point2D.FacingToPointVector.Select( facingKv => {
                var step = facingKv.Value;
                var point = treeAt;
                var height = kv.Value;
                int score = 0;
                while (treeMap.TryGetValue(point += step, out var next))
                {
                    score++;
                    if (next >= height) break;
                }
                return score;
            }).Aggregate(1, (a, b) => a * b);
        }).Max();
        Console.WriteLine(r);
    }
}