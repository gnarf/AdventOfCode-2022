using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace AoC2022;

class Day20 : Puzzle
{
    public List<int> initial = new();

    public override void Parse(string filename)
    {
        base.Parse(filename);
        initial = lines.Select(l => int.Parse(l)).ToList();
    }

    public override void Part1()
    {
        // Working list stores an index into initial...
        // a value of 1 in this list corresponds to the value at initial[1], etc
        List<int> working = Enumerable.Range(0, initial.Count).ToList();

        int val(int index) => initial[working[index % initial.Count]];

        // loop inital to get ordering.
        for (int i = 0; i<initial.Count; i++)
        {
            var index = working.IndexOf(i);
            var moveTo = ( index + val(index) ) % (working.Count - 1);
            if (moveTo <= 0) moveTo = working.Count + moveTo - 1;
            Console.WriteLine($"{i}: Moving {val(i)} from {index} to {moveTo}");
            // Console.WriteLine(working.Aggregate("", (a, b) => a+val(b)+", "));
            working.Insert(moveTo + (moveTo > index ? 1 : 0), i);
            // Console.WriteLine(working.Aggregate("", (a, b) => a+val(b)+", "));
            working.RemoveAt(index + (moveTo < index ? 1 : 0));
            // Console.WriteLine(working.Aggregate("", (a, b) => a+val(b)+", "));
        }

        var offset = working.IndexOf(initial.IndexOf(0));

        var result = val(offset + 1000) + val(offset + 2000) + val(offset + 3000);
        Console.WriteLine( $"{result}" );
    }

    public override void Part2()
    {
        List<int> working = Enumerable.Range(0, initial.Count).ToList();

        long val(int index) => initial[working[index % initial.Count]] * 811589153L;

        // loop inital to get ordering.
        for (int x = 0; x<10; x++) for (int i = 0; i<initial.Count; i++)
        {
            var index = working.IndexOf(i);
            var moveTo = (int)(( index + val(index) ) % (working.Count - 1));
            if (moveTo <= 0) moveTo = working.Count + moveTo - 1;
            // Console.WriteLine($"{i}: Moving {val(i)} from {index} to {moveTo}");
            // Console.WriteLine(working.Aggregate("", (a, b) => a+val(b)+", "));
            working.Insert(moveTo + (moveTo > index ? 1 : 0), i);
            // Console.WriteLine(working.Aggregate("", (a, b) => a+val(b)+", "));
            working.RemoveAt(index + (moveTo < index ? 1 : 0));
            // Console.WriteLine(working.Aggregate("", (a, b) => a+val(b)+", "));
        }

        var offset = working.IndexOf(initial.IndexOf(0));

        var result = val(offset + 1000) + val(offset + 2000) + val(offset + 3000);
        Console.WriteLine( $"{result}" );
    }
}