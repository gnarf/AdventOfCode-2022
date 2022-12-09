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
        Point2D[] knots = Enumerable.Repeat(new Point2D(0, 0), 10).ToArray();
        HashSet<Point2D> tailVisit = new HashSet<Point2D>{Point2D.Zero};

        void moveKnot(int index, Point2D move)
        {
            knots[index] += move;
            if (index == knots.Length - 1)
            {
                tailVisit.Add(knots[index]);
            }
            else
            {
                var dist = knots[index] - knots[index + 1];
                if (dist != Point2D.Sign(dist))
                {
                    moveKnot(index + 1, Point2D.Sign(dist));
                }
            }
        }

        foreach (var move in GetMoves())
        {
            for (int x=0; x<move.steps; x++)
            {
                moveKnot(0, Point2D.FacingToPointVector[move.dir]);
            }
        }
        Console.WriteLine(tailVisit.Count);
    }

    /// What does the distribution of knot length look like?
    public void Bonus()
    {
        Console.WriteLine("--- bonus content ---");
        List<(Point2D pos, HashSet<Point2D> seen)> knots = new()
        {
            (Point2D.Zero, new(){Point2D.Zero})
        };
        void moveKnot(int index, Point2D move)
        {
            var (pos, seen) = knots[index];
            pos += move;
            seen.Add(pos);
            knots[index] = (pos, seen);
            if (index == knots.Count - 1)
            {
                knots.Add( (Point2D.Zero, new(){Point2D.Zero}) );
            }
            var dist = pos - knots[index + 1].pos;
            if (dist != Point2D.Sign(dist))
            {
                moveKnot(index + 1, Point2D.Sign(dist));
            }
        }
        foreach (var move in GetMoves())
        {
            for (int x=0; x<move.steps; x++)
            {
                moveKnot(0, Point2D.FacingToPointVector[move.dir]);
            }
        }

        for (int x = 0; x<knots.Count; x++)
        {
            var z = knots[0].seen.Count();
            var c = knots[x].seen.Count();
            var bar = "".PadLeft((int)((double)c * 80 / (double)z), '#');
            Console.WriteLine($"{x.ToString().PadLeft(3,' ')} {knots[x].seen.Count().ToString().PadLeft(4,' ')} {bar}");
        }

    }
}