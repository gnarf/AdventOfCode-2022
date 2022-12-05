using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
namespace AoC2022;

class Day5 : Puzzle
{

    private int tableBottom;

    /// <summary>
    /// Returns a array of Stack<char> representing the game state from the input file.
    /// index 0 of the array is empty intentionally to make working with "from index 1" pointing at index 1.
    /// </summary>
    Stack<char>[] getInitialState()
    {
        tableBottom = Array.FindIndex(lines, s=> s==" 1   2   3   4   5   6   7   8   9 ");

        Stack<char>[] state = Enumerable.Repeat(0, 10).Select((s)=>new Stack<char>()).ToArray();
        for (int x=tableBottom - 1; x >= 0; x--)
        {
            lines[x] = lines[x].PadRight(30,' ');
            for (int y=1; y<10; y++)
            {
                var c = lines[x][(y*4) - 3];
                if (c != ' ')
                    state[y].Push(c);
            }
        }

        return state;
    }

    IEnumerable<(int count, int fromStack, int toStack)> getGameMoves()
    {
        for (int x=tableBottom+2; x< lines.Length; x++)
        {
            var parts = lines[x].Split(' ');
            int count = Convert.ToInt32(parts[1]);
            int fromStack = Convert.ToInt32(parts[3]);
            int toStack = Convert.ToInt32(parts[5]);
            yield return (count, fromStack, toStack);
        }
    }

    public override void Part1()
    {
        var stacks = getInitialState();
        foreach ((int count, int fromStack, int toStack) in getGameMoves())
        {
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
        var stacks = getInitialState();
        var swap = new Stack<char>(); // temp buffer to hold the swapping stack 
        foreach ((int count, int fromStack, int toStack) in getGameMoves())
        {
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