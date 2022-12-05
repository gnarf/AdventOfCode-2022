using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
namespace AoC2022;

class Day5 : Puzzle
{

    public Stack<char>[] stacks = new Stack<char>[10];
    public override void Part1()
    {
        var list = lines.ToList();
        var tableBottom = list.FindIndex(s=> s==" 1   2   3   4   5   6   7   8   9 ");

        for (int x=tableBottom - 1; x >= 0; x--)
        {
            list[x] = list[x].PadRight(30,' ');
            for (int y=1; y<10; y++)
            {
                stacks[y] ??= new Stack<char>();
                var c = list[x][(y*4) - 3];
                if (c != ' ')
                    stacks[y].Push(c);
            }

        }
        
        for (int x=tableBottom+2; x< list.Count; x++)
        {
            var parts = list[x].Split(' ');
            int count = Convert.ToInt32(parts[1]);
            int fromStack = Convert.ToInt32(parts[3]);
            int toStack = Convert.ToInt32(parts[5]);
            for (int y=0; y<count;y++)
            {
                stacks[toStack].Push(stacks[fromStack].Pop());
            }
        }

        var result = stacks.Skip(1).Aggregate("", (string a, Stack<char> b) => a+b.Pop());

        Console.WriteLine(result);
    }

    public override void Part2()
    {
        var list = lines.ToList();
        var tableBottom = list.FindIndex(s=> s==" 1   2   3   4   5   6   7   8   9 ");

        for (int x=tableBottom - 1; x >= 0; x--)
        {
            list[x] = list[x].PadRight(30,' ');
            for (int y=1; y<10; y++)
            {
                stacks[y] ??= new Stack<char>();
                var c = list[x][(y*4) - 3];
                if (c != ' ')
                    stacks[y].Push(c);
            }

        }
        
        var swap = new Stack<char>();
        for (int x=tableBottom+2; x< list.Count; x++)
        {
            var parts = list[x].Split(' ');
            int count = Convert.ToInt32(parts[1]);
            int fromStack = Convert.ToInt32(parts[3]);
            int toStack = Convert.ToInt32(parts[5]);
            for (int y=0; y<count;y++)
            {
                swap.Push(stacks[fromStack].Pop());
            }
            for (int y=0; y<count;y++)
            {
                stacks[toStack].Push(swap.Pop());
            }
        }

        var result = stacks.Skip(1).Aggregate("", (string a, Stack<char> b) => a+b.Pop());

        Console.WriteLine(result);
    }
}