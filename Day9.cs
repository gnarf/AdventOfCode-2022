using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
namespace AoC2022;

class Day9 : Puzzle
{

    IEnumerable<(Facing2D dir, int steps)> GetMoves()
    {
        foreach(var line in lines)
        {
            var parts = line.Split(' ');
            yield return (parts[0] switch { "R" => Facing2D.Right, "U" => Facing2D.Up, "D" => Facing2D.Down, _ => Facing2D.Left}, Convert.ToInt32(parts[1]));
        }
    }

    public override void Part1()
    {
        Point2D head = new Point2D(0, 0);
        Point2D tail = new Point2D(0, 0);
        HashSet<Point2D> tailVisit = new HashSet<Point2D>{tail};
        foreach (var move in GetMoves())
        {
            for (int x=0; x<move.steps; x++)
            {
                head += Point2D.FacingToPointVector[move.dir];
                var dist = head - tail;
                if (dist != Point2D.Sign(dist))
                {
                    tail += Point2D.Sign(dist);
                    tailVisit.Add(tail);
                }
                // Console.WriteLine($"Step {move.dir} {head} {tail}");
            }
        }
        Console.WriteLine(tailVisit.Count);
    }

    public override void Part2()
    {
        Point2D[] heads = Enumerable.Repeat(new Point2D(0, 0), 9).ToArray();
        Point2D[] tails = Enumerable.Repeat(new Point2D(0, 0), 9).ToArray();
        HashSet<Point2D> tailVisit = new HashSet<Point2D>{Point2D.zero};

        void moveHead(int index, Point2D move)
        {
            heads[index] += move;
            var dist = heads[index] - tails[index];
            if (dist != Point2D.Sign(dist))
            {
                tails[index] += Point2D.Sign(dist);
                if (index < heads.Length - 1)
                {
                    moveHead(index + 1, Point2D.Sign(dist));
                }
                else
                {
                    tailVisit.Add(tails[index]);
                }
            }

        }

        foreach (var move in GetMoves())
        {
            for (int x=0; x<move.steps; x++)
            {
                moveHead(0, Point2D.FacingToPointVector[move.dir]);
            }
        }
        Console.WriteLine(tailVisit.Count);
    }
}