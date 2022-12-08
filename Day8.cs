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
            int[] MaxPass = new int[4]{-1,-1,-1,-1};
            for (int y=0; y<lines.Length; y++)
            {
                Point2D[] points = new Point2D[]
                {
                    new Point2D(x, y),
                    new Point2D(y, x),
                    new Point2D(x, swap - y),
                    new Point2D(swap - y, x),
                };
                for (int z = 0; z<4; z++)
                {
                    var h = treeMap[points[z]];
                    if (h > MaxPass[z]) 
                    {
                        // Console.WriteLine($"{z} {points[z]} {h}");
                        seenTrees.Add(points[z]);
                        MaxPass[z] = h;
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
            var h = kv.Value;
            int[] scores = new int[] {0, 0, 0, 0};
            for (int x=kv.Key.x + 1; x<count; x++)
            {
                var np = new Point2D(x, kv.Key.y);
                var nv = treeMap[np];
                scores[0]++; 
                if (nv < h) { }
                else break;
            }
            for (int x=kv.Key.x - 1; x>=0; x--)
            {
                var np = new Point2D(x, kv.Key.y);
                var nv = treeMap[np];
                scores[1]++; 
                if (nv < h) { }
                else break;
            }
            for (int x=kv.Key.y + 1; x<count; x++)
            {
                var np = new Point2D(kv.Key.x, x);
                var nv = treeMap[np];
                scores[2]++;
                if (nv < h) {  }
                else break;
            }
            for (int x=kv.Key.y - 1; x>=0; x--)
            {
                var np = new Point2D(kv.Key.x, x);
                var nv = treeMap[np];
                scores[3]++;
                if (nv < h) {  }
                else break;
            }
            // Console.WriteLine($"{kv.Key} {scores[0]} {scores[1]} {scores[2]} {scores[3]} => {scores[0]*scores[1]*scores[2]*scores[3]}");
            return scores[0]*scores[1]*scores[2]*scores[3];
        }).Max();
        Console.WriteLine(r);
    }
}