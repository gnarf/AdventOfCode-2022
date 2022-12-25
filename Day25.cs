using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
namespace AoC2022;

class Day25 : Puzzle
{

    Dictionary<char, long> vals = new()
    {
        {'2', 2},
        {'1', 1},
        {'0', 0},
        {'-', -1},
        {'=', -2},
    };

    public long SNAFUToDec(string s)
    {
        long v = 0;
        for (int x=0; x<s.Length; x++)
        {
            v += vals[s[s.Length - x- 1]] * (long)Math.Pow(5, x);
        }
        return v;
    }

    public string DecToSNAFU(long d)
    {
        var result = "";

        long currentDigit = 1;
        long remaining = d;

        while (remaining != 0)
        {
            var v = (remaining % (currentDigit*5)) / currentDigit;
            if (v == 4)
            {
                result = "-" + result;
                remaining += 1 * (currentDigit);
            }
            else if (v == 3)
            {
                result = "=" + result;
                remaining += 2 * currentDigit;
            }
            else if (v == 2)
            {
                result = "2" + result;
                remaining -= 2 * currentDigit;
            }
            else if (v == 1)
            {
                result = "1" + result;
                remaining -= 1 * currentDigit;
            }
            else if (v == 0)
            {
                result = "0" + result;
                // remaining -= 1;
            }
            currentDigit *= 5;
        }
        return result;
    }

    // public override void Parse(string filename)
    // {
    //     base.Parse(filename);
    // }

    public override void Part1()
    {
        long runTotal = 0;
        foreach (var line in lines)
        {
            runTotal += SNAFUToDec(line);
        }
        Console.WriteLine(runTotal);
        Console.WriteLine(DecToSNAFU(runTotal));
    }

    // public override void Part2()
    // {
    // }
}