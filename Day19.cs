using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace AoC2022;

class Day19 : Puzzle
{

    public class RobotBlueprint
    {
        public string Type = "";
        public Resources resources = new();
    }

    public struct Resources
    {
        public int ore;
        public int clay;
        public int obsidian;
        public int geode;

        public Resources()
        {
        }

        public Resources(int ore, int clay, int obsidian, int geode)
        {
            this.ore = ore;
            this.clay = clay;
            this.obsidian = obsidian;
            this.geode = geode;
        }

        public Resources(Resources o)
        {
            this.ore = o.ore;
            this.clay = o.clay;
            this.obsidian = o.obsidian;
            this.geode = o.geode;
        }

        public static Resources operator +(Resources a, Resources b)
        {
            return new Resources(a.ore + b.ore, a.clay + b.clay, a.obsidian + b.obsidian, a.geode + b.geode);
        }
        public static Resources operator -(Resources a, Resources b)
        {
            return new Resources(a.ore - b.ore, a.clay - b.clay, a.obsidian - b.obsidian, a.geode - b.geode);
        }
        public static Resources operator *(Resources a, int b)
        {
            return new Resources(a.ore * b, a.clay * b, a.obsidian * b, a.geode * b);
        }

        public override string ToString()
        {
            return $"({ore}, {clay}, {obsidian}, {geode})";            
        }

        public bool LTE(Resources b)
        {
            return ore <= b.ore && clay <= b.clay && obsidian <= b.obsidian && geode <= b.geode;
        }
    }

    public List<Dictionary<string, RobotBlueprint>> Factories = new();

    public override void Parse(string filename)
    {
        base.Parse(filename);
        var re = new Regex(@"Each (?<Type>\w+) robot costs (?<Costs>[^\.]*)");
        var re2 = new Regex(@"(?<Cost>\d+) (?<Type>\w+)");
        foreach (var line in lines)
        {
            var matches = re.Match(line);
            var robots = new Dictionary<string, RobotBlueprint>();
            while (matches.Success)
            {
                var matchCost = re2.Match(matches.Groups["Costs"].Value);
                var bp = new RobotBlueprint{Type=matches.Groups["Type"].Value};

                while (matchCost.Success)
                {
                    var v = matchCost.Groups["Type"].Value switch
                    {
                        "ore" => bp.resources.ore = int.Parse(matchCost.Groups["Cost"].Value),
                        "obsidian" => bp.resources.obsidian = int.Parse(matchCost.Groups["Cost"].Value),
                        "clay" => bp.resources.clay = int.Parse(matchCost.Groups["Cost"].Value),
                        _ => throw new Exception()
                    };
                    matchCost = matchCost.NextMatch();
                }
                robots.Add(bp.Type, bp);
                matches = matches.NextMatch();
            }
            Factories.Add(robots);
        }
    }

    public struct FactoryState
    {
        public int minutesLeft;

        public Resources v = new(1, 0, 0,0);
        public Resources i = new();

        public FactoryState() {}
        public FactoryState(FactoryState copy)
        {
            minutesLeft = copy.minutesLeft;
            v = new Resources(copy.v);
            i = new Resources(copy.i);
        }

        public override string ToString()
        {
            return $"[T-{minutesLeft}] i:{i} v:{v}";
        }
    }


    // public override void Part1()
    // {
    //     int score = 0;
    //     int number = 1;
    //     int statesParsed = 0;

    //     foreach (var factory in Factories)
    //     {
    //         List<FactoryState> states = new();
    //         states.Add(new FactoryState{minutesLeft = 24});
    //         int maxgeodes = 0;
    //         while (states.Count > 0)
    //         {
    //             if (statesParsed++ % 1000000 == 0) TimeCheck($"States Parsed: {statesParsed} - {states.Count}");
    //             var state = states[states.Count - 1];
    //             states.RemoveAt(states.Count - 1);
    //             // Console.WriteLine(state);

    //             var doNothing = state.i.geode + state.v.geode*state.minutesLeft;
    //             if (doNothing > maxgeodes)
    //             {
    //                 maxgeodes = doNothing;
    //                 Console.WriteLine($"New max: {state} {maxgeodes}");
    //             }

    //             foreach (var bp in factory)
    //             {
    //                 int turnAvailable = state.minutesLeft;
    //                 var resources = state.i;
    //                 while (turnAvailable >= 0 && (resources.clay < bp.Value.resources.clay ||
    //                     resources.ore < bp.Value.resources.ore ||
    //                     resources.obsidian < bp.Value.resources.obsidian))
    //                 {
    //                     turnAvailable--;
    //                     resources += state.v;
    //                 }
    //                 turnAvailable--;
    //                 resources += state.v;
    //                 if (
    //                     turnAvailable > 0 &&
    //                     turnAvailable < state.minutesLeft &&
    //                     resources.clay >= bp.Value.resources.clay &&
    //                     resources.ore >= bp.Value.resources.ore &&
    //                     resources.obsidian >= bp.Value.resources.obsidian
    //                 )
    //                 {
    //                     var built = new FactoryState{minutesLeft=turnAvailable, v=state.v, i=resources};
    //                     var _ = bp.Key switch
    //                     {
    //                         "ore" => built.v.ore++,
    //                         "obsidian" => built.v.obsidian++,
    //                         "clay" => built.v.clay++,
    //                         "geode" => built.v.geode++,
    //                         _ => throw new Exception()
    //                     };
    //                     built.i -= bp.Value.resources;
    //                     // if (!states.Any(s => s.minutesLeft == built.minutesLeft && s.i.LTE(built.i) && s.v.LTE(built.v)))
    //                         states.Add(built);
    //                     if (bp.Key == "geode")
    //                     {
    //                         // Console.WriteLine($"Geode built {built}");
    //                     }
    //                 }
    //             }
    //         }
    //         var s = maxgeodes * number;
    //         score += s;
    //         Console.WriteLine($"F#{number}:{maxgeodes}  = {s}");
    //         number++;
    //     }
    //     Console.WriteLine(score);
    // }

    public override void Part2()
    {
        int score = 0;
        int number = 1;
        int statesParsed = 0;
        int lastMinute = -1;

        List<int> maxGeodes = new();

        foreach (var factory in Factories.Take(3))
        {
            List<FactoryState> states = new();
            states.Add(new FactoryState{minutesLeft = 32});
            int maxgeodes = 0;
            while (states.Count > 0)
            {
                if (lastMinute != states[0].minutesLeft)
                {
                    states.Sort((a, b) =>
                    {
                        int min = b.minutesLeft - a.minutesLeft;
                        if (min != 0) return min;
                        return 0;
                        // return b.v.LTE(a.v) && b.i.LTE(a.i) ? (a.v.LTE(b.v) && a.i.LTE(b.i) ? 0 : 1) : -1;
                    });
                    lastMinute = states[0].minutesLeft;
                    Console.WriteLine($"New minute: {states[0]}");

                }
                if (statesParsed++ % 10000 == 0) TimeCheck($"States Parsed: {statesParsed} - {states.Count} - {states[0].minutesLeft}");
                var state = states[0];
                states.RemoveAt(0);
                // Console.WriteLine(state);

                var doNothing = state.i.geode + state.v.geode*state.minutesLeft;
                if (doNothing > maxgeodes)
                {
                    maxgeodes = doNothing;
                    Console.WriteLine($"New max: {state} {maxgeodes}");
                }

                foreach (var bp in factory)
                {
                    int turnAvailable = state.minutesLeft;
                    var resources = state.i;
                    while (turnAvailable >= 0 && (resources.clay < bp.Value.resources.clay ||
                        resources.ore < bp.Value.resources.ore ||
                        resources.obsidian < bp.Value.resources.obsidian))
                    {
                        turnAvailable--;
                        resources += state.v;
                    }
                    turnAvailable--;
                    resources += state.v;
                    if (
                        turnAvailable > 0 &&
                        turnAvailable < state.minutesLeft &&
                        resources.clay >= bp.Value.resources.clay &&
                        resources.ore >= bp.Value.resources.ore &&
                        resources.obsidian >= bp.Value.resources.obsidian
                    )
                    {
                        var built = new FactoryState{minutesLeft=turnAvailable, v=state.v, i=resources};
                        var _ = bp.Key switch
                        {
                            "ore" => built.v.ore++,
                            "obsidian" => built.v.obsidian++,
                            "clay" => built.v.clay++,
                            "geode" => built.v.geode++,
                            _ => throw new Exception()
                        };
                        built.i -= bp.Value.resources;
                        // if (!states.Any(s => s.minutesLeft == built.minutesLeft && s.i.LTE(built.i) && s.v.LTE(built.v)))
                            states.Add(built);
                        if (bp.Key == "geode")
                        {
                            // Console.WriteLine($"Geode built {built}");
                        }
                    }
                }
            }
            var s = maxgeodes * number;
            maxGeodes.Add(maxgeodes);
            score += s;
            Console.WriteLine($"F#{number}:{maxgeodes}  = {s}");
            number++;
        }
        Console.WriteLine(maxGeodes.Aggregate(1, (a,b) => a*b));
    }
}