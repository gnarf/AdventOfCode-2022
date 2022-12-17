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
            var coords = line.Split(new char[]{' ', '-', '>', ','}, StringSplitOptions.RemoveEmptyEntries);
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

    public List<Point2D> FallingOrder = new() { Point2D.Down, Point2D.Down + Point2D.Left, Point2D.Down + Point2D.Right};

    public int rested = 0;

    public override void Part1()
    {
        long puzzleBottom = puzzleState.Keys.Aggregate(Point2D.Max).y + 1;

        while (true)
        {
            var falling = new Point2D(500,0);
            try
            {
                int index = -1;
                while ((index = FallingOrder.FindIndex(d => GetState(falling + d) == State.Empty)) != -1)
                {
                    falling += FallingOrder[index];
                    // Console.WriteLine("Falls to "+falling);
                    if (falling.y >= puzzleBottom) break;
                }
            }
            catch (InvalidOperationException) {} // expected
            if (falling.y >= puzzleBottom) break;
            rested++;
            puzzleState[falling] = State.Sand;
            // Console.WriteLine("Stopped falling at" + falling);
        }

        Console.WriteLine("Rested "+rested);
    }

    public override void Part2()
    {

        long puzzleBottom = puzzleState.Keys.Aggregate(Point2D.Max).y + 2;

        while (true)
        {
            var falling = new Point2D(500,0);
            if (GetState(falling) == State.Sand) break;
            try
            {
                int index = -1;
                while ((index = FallingOrder.FindIndex(d => GetState(falling + d) == State.Empty)) != -1)
                {
                    // cant fall to puzzlebottom
                    if (falling.y == puzzleBottom - 1) break;
                    falling += FallingOrder[index];
                    // Console.WriteLine("Falls to "+falling);
                    if (falling.y >= puzzleBottom) break;
                }
            }
            catch (InvalidOperationException) {} // expected
            rested++;
            puzzleState[falling] = State.Sand;
            // Console.WriteLine("Stopped falling at" + falling);
        }

        Console.WriteLine("Rested "+rested);

    }
}