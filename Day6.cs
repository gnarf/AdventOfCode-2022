using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
namespace AoC2022;

class Day6 : Puzzle
{
    [TestCase("bvwbjplbgvbhsrlpgdmjqwftvncz", ExpectedResult = 5)]
    public static int FirstMarker(string s)
    {
        for (int x=4; x<s.Length; x++)
        {
            if (s.Substring(x-4, 4).ToHashSet().Count() == 4) return x;
        }
        return -1;
    }
    [TestCase("mjqjpqmgbljsphdztnvjfqwrcgsmlb", ExpectedResult = 19)]
    public static int SecondMarker(string s)
    {
        for (int x=14; x<s.Length; x++)
        {
            if (s.Substring(x-14, 14).ToHashSet().Count() == 14) return x;
        }
        return -1;
    }

    public override void Part1()
    {
        Console.WriteLine(FirstMarker(lines[0]));
    }

    public override void Part2()
    {
        Console.WriteLine(SecondMarker(lines[0]));
    }
}