using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
namespace AoC2022;

class Day10 : Puzzle
{

    public enum Command { noop, addx }
    public IEnumerable<(Command, int?)> GetCommands()
    {
        foreach(var line in lines)
        {
            var parts = line.Split(' ');
            yield return parts[0] switch
            {
                "noop" => (Command.noop, null),
                "addx" => (Command.addx, int.Parse(parts[1])),
                _ => throw new Exception(),
            };
        }
    }

    public override void Part1()
    {
        int cycle = 0;
        int X = 1;
        int strength = 0;
        void doCycle(int cycles)
        {
            var nextCycle = cycle + cycles;
            if (cycle/20 < nextCycle/20 && (nextCycle/20 % 2 == 1))
            {
                strength += (nextCycle/20)*20 * X;
            }
            cycle = nextCycle;
        }
        foreach (var (command, num) in GetCommands())
        {
            if (command == Command.noop)
            {
                doCycle(1);
            }
            else
            if (command == Command.addx)
            {
                doCycle(2);
                X += num ?? 0;
            }
            
        }
        Console.WriteLine(strength);
    }

    public override void Part2()
    {
        int cycle = 0;
        int X = 1;
        Dictionary<Point2D, bool> screen = new();
        void doCycle(int cycles)
        {
            while (cycles-- > 0)
            {
                var x = cycle % 40;
                var y = cycle / 40;
                screen.Add(new Point2D(x, y), Math.Abs(X-x) <= 1);
                cycle++;
            }
        }
        foreach (var (command, num) in GetCommands())
        {
            if (command == Command.noop)
            {
                doCycle(1);
            }
            else
            if (command == Command.addx)
            {
                doCycle(2);
                X += num ?? 0;
            }
            
        }
        for (int y=0; y<6; y++)
        {
            var S = "";
            for (int sx = 0; sx < 40; sx++) S += screen[new Point2D(sx, y)] ? '#' : '.';
            Console.WriteLine(S);
        }
     }
}