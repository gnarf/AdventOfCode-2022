using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

using static AoC2022.Point2D.Point2DCarinals;
namespace AoC2022;

class Day23 : Puzzle
{

    public static List<Point2D> InitialElves = new();

    public override void Parse(string filename)
    {
        base.Parse(filename);
        for (int y = 0; y<lines.Length; y++)
        {
            for (int x=0; x<lines[y].Length; x++)
            {
                if (lines[y][x] == '#') InitialElves.Add(new Point2D(x, y));
            }
        }
    }

    public override void Part1()
    {
        var elves = new List<Point2D>(InitialElves);
        // bool sawElf = true;

        for (int loop = 0; loop<10; loop++)
        {
            // first half of round elves propose a move:
            // sawElf = false;
            Dictionary<Point2D, Point2D> proposedMoves = new();
            foreach (var elf in elves)
            {
                // Console.Write(elf);
                if (!elves.Except(Enumerable.Repeat(elf, 1)).Any( e2 => Point2D.Sign(e2-elf) == e2 - elf))
                {
                    // Console.WriteLine(" chillin'");
                    
                }
                else
                {
                    for (int test = 0; test < 4; test++)
                    {
                        if ((test + loop)%4 == 0 && !(elves.Contains(elf + North) || elves.Contains(elf + North + West) || elves.Contains(elf + North + East)))
                        {
                            // Console.WriteLine(" north");
                            proposedMoves.Add(elf, elf + North);
                            break;
                        }
                        if ((test + loop)%4 == 1 && !(elves.Contains(elf + South) || elves.Contains(elf + South + West) || elves.Contains(elf + South + East)))
                        {
                            // Console.WriteLine(" south");
                            proposedMoves.Add(elf, elf + South);
                            break;
                        }
                        if ((test + loop)%4 == 2 && !(elves.Contains(elf + West) || elves.Contains(elf + West + South) || elves.Contains(elf + West + North)))
                        {
                            // Console.WriteLine(" west");
                            proposedMoves.Add(elf, elf + West);
                            break;
                        }
                        if ((test + loop)%4 == 3 && !(elves.Contains(elf + East) || elves.Contains(elf + East + South) || elves.Contains(elf + East + North)))
                        {
                            // Console.WriteLine(" east");
                            proposedMoves.Add(elf, elf + East);
                            break;
                        }
                    }
                }

            }
            // if (!sawElf) break;
            foreach (var kv in proposedMoves)
            {
                if (proposedMoves.Any(kv2 => kv2.Value == kv.Value && kv.Key != kv2.Key)) continue;
                // Console.WriteLine($"Elf {kv.Key} move {kv.Value}");
                elves.RemoveAll(e => e == kv.Key);
                elves.Add(kv.Value);
            }
            var min = elves.Aggregate(Point2D.Min);
            var max = elves.Aggregate(Point2D.Max);
            var count = 0;
            for (long y = min.y; y <= max.y; y++)
            {
                var s = "";
                for (long x = min.x; x <= max.x; x++)
                {
                    var hasElf = elves.Contains(new Point2D(x, y));
                    s += hasElf ? '#' : '.';
                    if (!hasElf) count++;
                }
                Console.WriteLine(s);
            }
            Console.WriteLine(count);
            // Console.WriteLine(elves.Count);
        }

    }

    public override void Part2()
    {
        IEnumerable<int> testFour(int round) 
        {
            for(int x = 0; x<4; x++) yield return (x+round)%4;
        }
        var elves = new HashSet<Point2D>(InitialElves);
        bool sawElf = true;
        int loop = 0;
        Dictionary<Point2D, Point2D> proposedMoves = new();
        List<Point2D> eightLooksClockwise = new()
        {
            North + West, North, North + East, East, South + East, South, South + West, West
        };
        while (sawElf)
        {
            // first half of round elves propose a move:
            sawElf = false;
            proposedMoves.Clear();
            foreach (var elf in elves)
            {
                // Console.Write(elf);
                var saw = eightLooksClockwise.Select(l => elves.Contains(elf + l)).ToArray();
                if (!saw.Any(v=>v))
                {
                    // Console.WriteLine(" chillin'");

                }
                else
                {
                    sawElf = true;
                    foreach(int test in testFour(loop))
                    {
                        if (test == 0 && !(saw[0] || saw[1] || saw[2]))
                        {
                            // Console.WriteLine(" north");
                            proposedMoves.Add(elf, elf + North);
                            break;
                        }
                        if (test == 1 && !(saw[4] || saw[5] || saw[6]))
                        {
                            // Console.WriteLine(" south");
                            proposedMoves.Add(elf, elf + South);
                            break;
                        }
                        if (test == 2 && !(saw[6] || saw[7] || saw[0]))
                        {
                            // Console.WriteLine(" west");
                            proposedMoves.Add(elf, elf + West);
                            break;
                        }
                        if (test == 3 && !(saw[2] || saw[3] || saw[4]))
                        {
                            // Console.WriteLine(" east");
                            proposedMoves.Add(elf, elf + East);
                            break;
                        }
                    }
                }

            }
            // if (!sawElf) break;
            
            foreach (var kv in proposedMoves.GroupBy(kv => kv.Value).Where(kv => kv.Count() == 1).Select(kv => kv.First()))
            {
                // Console.WriteLine($"Elf {kv.Key} move {kv.Value}");
                elves.Remove(kv.Key);
                elves.Add(kv.Value);
            }
            // var min = elves.Aggregate(Point2D.Min);
            // var max = elves.Aggregate(Point2D.Max);
            // var count = 0;
            // for (long y = min.y; y <= max.y; y++)
            // {
            //     // var s = "";
            //     for (long x = min.x; x <= max.x; x++)
            //     {
            //         var hasElf = elves.Contains(new Point2D(x, y));
            //         // s += hasElf ? '#' : '.';
            //         if (!hasElf) count++;
            //     }
            //     // Console.WriteLine(s);
            // }
            loop++;
            TimeCheck($"turn {loop}");
            if (loop > 1e5) return;
            // Console.WriteLine(elves.Count);
        }
        var min = elves.Aggregate(Point2D.Min);
        var max = elves.Aggregate(Point2D.Max);
        var count = 0;
        for (long y = min.y; y <= max.y; y++)
        {
            var s = "";
            for (long x = min.x; x <= max.x; x++)
            {
                var hasElf = elves.Contains(new Point2D(x, y));
                s += hasElf ? '#' : '.';
                if (!hasElf) count++;
            }
            Console.WriteLine(s);
        }
        // 1017 is wrong
        TimeCheck($"No move for {loop}");

    }
}