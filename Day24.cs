using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
namespace AoC2022;

class Day24 : Puzzle
{

    public static List<(Point2D start, Point2D direction)> BlizzardStates = new();
    public static int gridWidth;
    public static int gridHeight;
    public static Point2D startPosition = new Point2D(0,-1);
    public static Point2D goalPosition => new Point2D(gridWidth - 1, gridHeight);

    public static bool Blocked(Point2D point, int turn)
    {
        if (point == startPosition || point == goalPosition) return false;
        if (point.x < 0 || point.y < 0 || point.x >= gridWidth || point.y >= gridHeight)
        {
            return true;
        }

        if (BlizzardStates.Any(b => point == (b.start + (b.direction * turn)).CycleMod(new Point2D(gridWidth, gridHeight)))) return true;

        return false;
    }

    public override void Parse(string filename)
    {
        base.Parse(filename);
        gridHeight = lines.Length - 2;
        gridWidth = lines[1].Length - 2;

        for (int i = 1; i < lines.Length - 1; i++)
        {
            for (int j = 1; j < lines[i].Length - 1; j++)
            {
                if (lines[i][j] == '>')
                {
                    BlizzardStates.Add((new Point2D(j-1, i-1), Point2D.Right));
                }
                if (lines[i][j] == 'v')
                {
                    BlizzardStates.Add((new Point2D(j-1, i-1), Point2D.Down));
                }
                if (lines[i][j] == '<')
                {
                    BlizzardStates.Add((new Point2D(j-1, i-1), Point2D.Left));
                }
                if (lines[i][j] == '^')
                {
                    BlizzardStates.Add((new Point2D(j-1, i-1), Point2D.Up));
                }
            }
        }
    }

    public override void Part1()
    {
        return;
        Queue<(Point2D position, int turn)> states = new();
        states.Enqueue((startPosition, 0));
        int stateTests = 0;
        int lastTurn = -1;
        // for (int turn=0; turn<19; turn++)
        // {
        //     Console.WriteLine($"---test turn {turn}---");
        //     Console.WriteLine("#." + "".PadLeft(gridWidth, '#'));
        //     for (int y=0; y<gridHeight; y++)
        //     {
        //         var s = "#";
        //         for (int x=0; x<gridWidth; x++)
        //         {
        //             var blizzards = BlizzardStates.Where(b => (b.start+(b.direction*turn)).CycleMod(new Point2D(gridWidth, gridHeight)) == new Point2D(x, y)).ToArray();
        //             if (blizzards.Length == 0) s += '.';
        //             if (blizzards.Length > 1) s += blizzards.Length;
        //             if (blizzards.Length == 1)
        //             {
        //                 if (blizzards[0].direction == Point2D.Right) s+='>';
        //                 if (blizzards[0].direction == Point2D.Left) s+='<';
        //                 if (blizzards[0].direction == Point2D.Up) s+='^';
        //                 if (blizzards[0].direction == Point2D.Down) s+='v';
        //             }
        //         }
        //         Console.WriteLine($"{s}#");
        //     }
        // }
        // return;
        var seenPositions = new HashSet<Point2D>();
        while (states.Count > 0)
        {
            var (position, turn) = states.Dequeue();
            if (turn != lastTurn)
            {
                TimeCheck($"Turn {turn} {states.Count}");
                seenPositions.Clear();
                lastTurn = turn;
            }
            turn++;
            if (seenPositions.Contains(position)) continue;
            seenPositions.Add(position);

            if (stateTests++ % 10000 == 0) TimeCheck($"{stateTests} ({position},{turn}) {states.Count}");
            // stall
            if (!Blocked(position, turn)) states.Enqueue((position, turn));
            foreach (var direction in Point2D.FacingToPointVector.Values)
            {
                var next = position + direction;
                if (next == goalPosition)
                {
                    Console.WriteLine($"Game ended on turn {turn}");
                    return;
                }
                if (!Blocked(next, turn)) states.Enqueue((next, turn));
            }
        }
        Console.WriteLine();
    }

    public override void Part2()
    {
        Queue<(Point2D position, int turn, int goalState)> states = new();
        states.Enqueue((startPosition, 0, 0));
        int stateTests = 0;
        int lastTurn = -1;
        int bestGoalState = 0;
        // for (int turn=0; turn<19; turn++)
        // {
        //     Console.WriteLine($"---test turn {turn}---");
        //     Console.WriteLine("#." + "".PadLeft(gridWidth, '#'));
        //     for (int y=0; y<gridHeight; y++)
        //     {
        //         var s = "#";
        //         for (int x=0; x<gridWidth; x++)
        //         {
        //             var blizzards = BlizzardStates.Where(b => (b.start+(b.direction*turn)).CycleMod(new Point2D(gridWidth, gridHeight)) == new Point2D(x, y)).ToArray();
        //             if (blizzards.Length == 0) s += '.';
        //             if (blizzards.Length > 1) s += blizzards.Length;
        //             if (blizzards.Length == 1)
        //             {
        //                 if (blizzards[0].direction == Point2D.Right) s+='>';
        //                 if (blizzards[0].direction == Point2D.Left) s+='<';
        //                 if (blizzards[0].direction == Point2D.Up) s+='^';
        //                 if (blizzards[0].direction == Point2D.Down) s+='v';
        //             }
        //         }
        //         Console.WriteLine($"{s}#");
        //     }
        // }
        // return;
        var seenPositions = new HashSet<(Point2D, int)>();
        while (states.Count > 0)
        {
            var (position, turn, goalState) = states.Dequeue();
            if (turn != lastTurn)
            {
                TimeCheck($"Turn {turn} {position} {goalState} {seenPositions.Count}");
                seenPositions.Clear();
                lastTurn = turn;
            }
            turn++;
            if (goalState > bestGoalState) bestGoalState = goalState;
            if (goalState < bestGoalState) continue;
            if (seenPositions.Contains((position,goalState))) continue;
            seenPositions.Add((position,goalState));

            if (stateTests++ % 10000 == 0) TimeCheck($"{stateTests} ({position},{turn},{goalState}) {states.Count}");
            // stall
            if (!Blocked(position, turn)) states.Enqueue((position, turn, goalState));
            foreach (var direction in Point2D.FacingToPointVector.Values)
            {
                var next = position + direction;
                if (next == goalPosition && goalState == 0)
                {
                    states.Enqueue((next, turn, goalState + 1));
                }
                else
                if (next == startPosition && goalState == 1)
                {
                    states.Enqueue((next, turn, goalState+1));
                }
                else
                if (next == goalPosition && goalState == 2)
                {
                    Console.WriteLine($"Game ended on turn {turn}");
                    return;
                }
                else
                if (!Blocked(next, turn)) states.Enqueue((next, turn, goalState));
            }
        }
        Console.WriteLine();
    }
}