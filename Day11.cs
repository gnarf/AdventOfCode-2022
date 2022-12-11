using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
namespace AoC2022;

class Day11 : Puzzle
{

    public class Monkey
    {
        public int id;
        public List<long> items = new();
        public List<string> operation = new();
        public int testMod = 0;
        public int testTrue = 0;
        public int testFalse = 0;
        public long testCount = 0;
    }

    public override void Parse(string filename)
    {
        base.Parse(filename);

    }

    public override void Part1()
    {
        // each monkey input 7 lines long
        var splits = new char[]{' ', ',', ':'};

        string[] splitLine(int monkey, int line) => lines[(monkey*7) + line].Trim().Split(splits, StringSplitOptions.RemoveEmptyEntries);
        List<Monkey> monkeys = new();

        for (int x=0; x*7<lines.Length; x++)
        {
            if (x != int.Parse(splitLine(x, 0)[1])) throw new Exception("Input error");
            var current = new Monkey();
            current.id = x;
            var s = splitLine(x, 1);
            current.items = splitLine(x, 1).Skip(2).Select( s=>long.Parse(s) ).ToList();
            current.operation = splitLine(x, 2).Skip(3).ToList();
            current.testMod = int.Parse(splitLine(x, 3).Last());
            current.testTrue = int.Parse(splitLine(x, 4).Last());
            current.testFalse = int.Parse(splitLine(x, 5).Last());
            monkeys.Add(current);
        }

        for (int round = 0; round<20; round++)
        {
            for (int m = 0; m<monkeys.Count; m++)
            {
                var monkey = monkeys[m];
                var itemsStart = monkey.items.ToList();
                monkey.items.Clear();
                foreach (var item in itemsStart)
                {
                    var newWorry = monkey.operation switch {
                        ["old", "*", "old"] => item * item,
                        ["old", "*", var s] => item * int.Parse(s),
                        ["old", "+", var s] => item + int.Parse(s),
                        _ => throw new Exception("parsing operation")
                    };
                    Console.WriteLine($"R:{round} M:{m} I:{item} N:{newWorry} 3:{newWorry/3}");
                    newWorry /= 3;
                    monkey.testCount++;
                    if ( (newWorry % monkey.testMod) == 0)
                    {
                        monkeys[monkey.testTrue].items.Add(newWorry);
                    }
                    else
                    {
                        monkeys[monkey.testFalse].items.Add(newWorry);
                    }
                }
            }
        }

        monkeys.Sort((a,b) => (int)(b.testCount - a.testCount));

        Console.WriteLine(monkeys[0].testCount*monkeys[1].testCount);
    }

    public override void Part2()
    {

        // each monkey input 7 lines long
        var splits = new char[]{' ', ',', ':'};

        string[] splitLine(int monkey, int line) => lines[(monkey*7) + line].Trim().Split(splits, StringSplitOptions.RemoveEmptyEntries);
        List<Monkey> monkeys = new();



        for (int x=0; x*7<lines.Length; x++)
        {
            if (x != int.Parse(splitLine(x, 0)[1])) throw new Exception("Input error");
            var current = new Monkey();
            current.id = x;
            var s = splitLine(x, 1);
            current.items = splitLine(x, 1).Skip(2).Select( s=>long.Parse(s) ).ToList();
            current.operation = splitLine(x, 2).Skip(3).ToList();
            current.testMod = int.Parse(splitLine(x, 3).Last());
            current.testTrue = int.Parse(splitLine(x, 4).Last());
            current.testFalse = int.Parse(splitLine(x, 5).Last());
            monkeys.Add(current);
        }

        var globalMod = monkeys.Select( m => m.testMod ).Aggregate(1,(a, b) => a*b);
        Console.WriteLine(globalMod);

        for (int round = 0; round<10000; round++)
        {
            for (int m = 0; m<monkeys.Count; m++)
            {
                var monkey = monkeys[m];
                var itemsStart = monkey.items.ToList();
                monkey.items.Clear();
                foreach (var item in itemsStart)
                {
                    var newWorry = monkey.operation switch {
                        ["old", "*", "old"] => item * item,
                        ["old", "*", var s] => item * int.Parse(s),
                        ["old", "+", var s] => item + int.Parse(s),
                        _ => throw new Exception("parsing operation")
                    } % globalMod;
                    // newWorry /= 3;
                    monkey.testCount++;
                    if ( (newWorry % monkey.testMod) == 0)
                    {
                        monkeys[monkey.testTrue].items.Add(newWorry);
                    }
                    else
                    {
                        monkeys[monkey.testFalse].items.Add(newWorry);
                    }
                }
            }

            if (round%1000 == 999 || round < 20)
            {
                Console.WriteLine($"==Round {round}");  
                foreach(var m in monkeys)
                {
                    Console.WriteLine($"M{m.id} - {m.testCount}");
                }
            }
        }

        monkeys.Sort((a,b) => (int)(b.testCount - a.testCount));

        Console.WriteLine(monkeys[0].testCount*monkeys[1].testCount);

    }
}