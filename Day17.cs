using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AoC2022;

class Day17 : Puzzle
{

    // shape points are entered left->right bottom to top
    public List<List<Point2D>> Shapes = new()
    {
        // ####
        new()
        {
            new Point2D(0, 0),
            new Point2D(1, 0),
            new Point2D(2, 0),
            new Point2D(3, 0),
        },
        // .#.
        // ###
        // .#.
        new()
        {
            new Point2D(1, 0),
            new Point2D(0, -1),
            new Point2D(1, -1),
            new Point2D(2, -1),
            new Point2D(1, -2),
        },
        // ..#
        // ..#
        // ###
        new()
        {
            new Point2D(0, 0),
            new Point2D(1, 0),
            new Point2D(2, 0),
            new Point2D(2, -1),
            new Point2D(2, -2),
        },
        // #
        // #
        // #
        // #
        new()
        {
            new Point2D(0, 0),
            new Point2D(0, -1),
            new Point2D(0, -2),
            new Point2D(0, -3),
        },
        // ##
        // ##
        new()
        {
            new Point2D(0, 0),
            new Point2D(1, 0),
            new Point2D(0, -1),
            new Point2D(1, -1),
        },  
    };

    public IEnumerable<List<Point2D>> ShapeStream()
    {
        while (true) foreach (var shape in Shapes) yield return shape;
    }

    public IEnumerable<Point2D> JetStream()
    {
        while (true)
        {
            for (int x=0; x<lines[0].Length; x++)
            {
                yield return lines[0][x] switch {
                    '<' => Point2D.Left,
                    '>' => Point2D.Right,
                    _ => throw new Exception("unknown jet direction in input")
                };
            }
        }
    }

    public HashSet<Point2D> stoppedRocks = new();

    public bool ValidPoint(Point2D p)
    {
        // above the floor, away from walls.
        var checkone = p.y <= 0 && p.x >=0 && p.x <=6;
        if (!checkone) return false;
        return !stoppedRocks.Contains(p);
    }

    // public override void Parse(string filename)
    // {
    //     base.Parse(filename);
    // }

    public override void Part1()
    {
        var shapes = ShapeStream().GetEnumerator();
        var jets = JetStream().GetEnumerator();
        for (long x = 0; x<2022; x++)
        {
            // up is negative in this setup, 1 is the floor, just in case
            var maxHeight = stoppedRocks.Select(p => p.y).Append(1).Min();

            // three units "above"
            var rockOrigin = new Point2D(2, maxHeight - 4);
            shapes.MoveNext();
            var shape = new List<Point2D>(shapes.Current);

            while (true)
            {
                jets.MoveNext();
                var jet = jets.Current;
                // push by jet
                if (shape.All(p => ValidPoint(p+rockOrigin+jet)))
                {
                    Console.WriteLine($"{rockOrigin} Push shape {jet}");
                    rockOrigin += jet;
                } else { Console.WriteLine($"Push shape {jet} bumped, canceled"); }

                if (shape.All(p => ValidPoint(p+rockOrigin+Point2D.Down)))
                {
                    Console.WriteLine($"{rockOrigin} Shape falls");
                    rockOrigin += Point2D.Down;
                }
                else
                {
                    Console.WriteLine($"{rockOrigin} Shape comes to rest");
                    foreach (var p in shape) stoppedRocks.Add(p + rockOrigin);
                    // stoppedRocks.RemoveWhere(p => stoppedRocks.Any(p2 => p.x == p2.x && p.y > p2.y + 4));
                    break;
                }
            }
            // Point2D.PrintGrid(stoppedRocks, p => stoppedRocks.Contains(p) ? '#' : ' ');            
        }
        Console.WriteLine(1-stoppedRocks.Select(y => y.y).Min());
    }

    public override void Part2()
    {
        stoppedRocks.Clear();
        var shapes = ShapeStream().GetEnumerator();
        var jets = JetStream().GetEnumerator();
        int sequenceGuess = 35;
        long? sequenceLength = null;
        long? sequenceDiff = null;
        long? sequenceIndex = null;
        List<long> maxHeights = new();
        long lastRock = 1000000000000 - 1;
        for (long x = 0; x<1000000000000; x++)
        {
            // up is negative in this setup, 1 is the floor, just in case
            var maxHeight = stoppedRocks.Select(p => p.y).Append(1).Min();
            maxHeights.Add(maxHeight);
                if (maxHeights.Count > sequenceGuess * 3)
                {
                    HashSet<long> diffs = new();
                    for (long d=maxHeights.Count - sequenceGuess*2; d<maxHeights.Count - sequenceGuess; d++)
                    {
                        diffs.Add(maxHeights[(int)(d+sequenceGuess)] - maxHeights[(int)d]);
                    }
                    if (diffs.Count != 1)
                    {
                        Console.WriteLine($"Rock {x} failing sequence guess {sequenceGuess} {diffs.Count}");
                        sequenceGuess++;
                    }
                    else
                    {
                        // Console.WriteLine($"Confidence increasing");
                        if (maxHeights.Count > sequenceGuess * 10)
                        {
                            sequenceDiff = diffs.First();
                            sequenceLength = sequenceGuess;
                            Console.WriteLine($"Confident that max heights repeat every {sequenceLength} falling rocks. {sequenceDiff}");
                            sequenceIndex = lastRock % sequenceLength;
                        }
                    }
                }
            if (sequenceLength != null && (x + 1) % sequenceLength == sequenceIndex)
            {
                var cyclesToFinish = (lastRock - x - 1) / sequenceLength;
                var myheight = maxHeight + sequenceDiff*cyclesToFinish;
                Console.WriteLine($"IndexMatch: Answer is {sequenceLength}: {1-myheight}");
                return;
            }
            if (x%1e5 == 0) TimeCheck($"Loop {x} {maxHeight}");

            // three units "above"
            var rockOrigin = new Point2D(2, maxHeight - 4);
            shapes.MoveNext();
            var shape = new List<Point2D>(shapes.Current);

            while (true)
            {
                jets.MoveNext();
                var jet = jets.Current;
                // push by jet
                if (shape.All(p => ValidPoint(p+rockOrigin+jet)))
                {
                    rockOrigin += jet;
                } else {}

                if (shape.All(p => ValidPoint(p+rockOrigin+Point2D.Down)))
                {
                    rockOrigin += Point2D.Down;
                }
                else
                {
                    foreach (var p in shape) stoppedRocks.Add(p + rockOrigin);
                    // stoppedRocks.RemoveWhere(p => stoppedRocks.Any(p2 => p.x == p2.x && p.y > p2.y + 4));
                    break;
                }
            }
        }
        Console.WriteLine(1-stoppedRocks.Select(y => y.y).Min());
    }
}