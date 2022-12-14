using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace AoC2022;

class Day14 : Puzzle
{
    public enum State { Empty, Rock, Sand }
    public Dictionary<Point2D, State> puzzleState = new();

    public State GetState(Point2D point) => puzzleState.TryGetValue(point, out var result) ? result : State.Empty;

    public override void Parse(string filename)
    {
        base.Parse(filename);
        puzzleState.Clear();
        foreach( var line in lines)
        {
            var coords = line.Split(new char[]{' ', '-', '>', ','}, StringSplitOptions.RemoveEmptyEntries);            Point2D? lastPoint = null;
            for (int c=2; c<coords.Length; c+=2)
            {
                var prevCood = new Point2D(int.Parse(coords[c-2]), int.Parse(coords[c-1]));
                var coordPoint = new Point2D(int.Parse(coords[c]), int.Parse(coords[c+1]));
                var direction = Point2D.Sign(coordPoint - prevCood);
                for (var p = prevCood; p != coordPoint; p += direction)
                {
                    puzzleState[p] = State.Rock;
                }
                puzzleState[coordPoint] = State.Rock;
            }
        }
    }

    public Point2D[] FallingOrder = new Point2D[] { Point2D.Down, Point2D.Down + Point2D.Left, Point2D.Down + Point2D.Right};

    public int rested = 0;

    public override void Part1()
    {
        int puzzleBottom = puzzleState.Keys.Aggregate(Point2D.Max).y + 1;

        while (true)
        {
            var falling = new Point2D(500,0);
            Point2D fall;
            try
            {
                while ((fall = FallingOrder.First(d => GetState(falling + d) == State.Empty)) != Point2D.Zero)
                {
                    falling += fall;
                    // Console.WriteLine("Falls to "+falling);
                    if (falling.y >= puzzleBottom) break;
                }
            }
            catch (InvalidOperationException e) {} // expected
            if (falling.y >= puzzleBottom) break;
            rested++;
            puzzleState[falling] = State.Sand;
            Console.WriteLine("Stopped falling at" + falling);
        }

        Console.WriteLine("Rested "+rested);
    }

    public override void Part2()
    {

        int puzzleBottom = puzzleState.Keys.Aggregate(Point2D.Max).y + 2;

        while (true)
        {
            var falling = new Point2D(500,0);
            if (GetState(falling) == State.Sand) break;
            Point2D fall;
            try
            {
                while ((fall = FallingOrder.First(d => GetState(falling + d) == State.Empty)) != Point2D.Zero)
                {
                    // cant fall to puzzlebottom
                    if (falling.y == puzzleBottom - 1) break;
                    falling += fall;
                    // Console.WriteLine("Falls to "+falling);
                    if (falling.y >= puzzleBottom) break;
                }
            }
            catch (InvalidOperationException e) {} // expected
            rested++;
            puzzleState[falling] = State.Sand;
            Console.WriteLine("Stopped falling at" + falling);
        }

        Console.WriteLine("Rested "+rested);

    }
}