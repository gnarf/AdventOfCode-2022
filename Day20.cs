using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace AoC2022;

class Day20 : Puzzle
{

    public class Entry
    {
        public int originalPosition;
        public long value;
    }

    public List<Entry> initial = new();

    public override void Parse(string filename)
    {
        base.Parse(filename);
        int index = 0;
        foreach (var line in lines)
        {
            initial.Add(
                new Entry{originalPosition = index++, value = long.Parse(line)}
            );
        }
    }

    public override void Part1()
    {
        List<Entry> working = new(initial);

        // loop inital to get ordering.
        foreach (var entry in initial)
        {
            var index = working.IndexOf(entry);
            var moveTo = ( index + (int)entry.value ) % (working.Count - 1);
            if (moveTo <= 0) moveTo = working.Count + moveTo - 1;
            Console.WriteLine($"{entry.originalPosition}: Moving {entry.value} from {index} to {moveTo}");
            // Console.WriteLine(working.Aggregate("", (a, b) => a+b.value+", "));
            working.Insert(moveTo + (moveTo > index ? 1 : 0), entry);
            // Console.WriteLine(working.Aggregate("", (a, b) => a+b.value+", "));
            working.RemoveAt(index + (moveTo < index ? 1 : 0));
            // Console.WriteLine(working.Aggregate("", (a, b) => a+b.value+", "));
        }

        var offset = working.FindIndex(p => p.value == 0);

        var result = working[(offset + 1000) % working.Count].value +
            working[(offset + 2000) % working.Count].value +
            working[(offset + 3000) % working.Count].value;

        Console.WriteLine( result );
    }

    public override void Part2()
    {
        List<Entry> working = new(initial);

        var decrypt = 811589153;

            // Console.WriteLine(working.Aggregate("", (a, b) => a+(b.value*decrypt)+", "));
        for (int x=0; x<10;x++)
        {
            // loop inital to get ordering.
            foreach (var entry in initial)
            {
                var index = working.IndexOf(entry);
                var moveMod = (entry.value * decrypt) % (working.Count - 1);
                var moveTo = ( index + (int)(moveMod) ) % (working.Count - 1);
                while (moveTo <= 0) moveTo = working.Count + moveTo - 1;
                Console.WriteLine($"{x}.{entry.originalPosition}: Moving {entry.value*decrypt} {moveMod} from {index} to {moveTo}");
                // Console.WriteLine(working.Aggregate("", (a, b) => a+(b.value*decrypt)+", "));
                working.Insert(moveTo + (moveTo > index ? 1 : 0), entry);
                // Console.WriteLine(working.Aggregate("", (a, b) => a+(b.value*decrypt)+", "));
                working.RemoveAt(index + (moveTo < index ? 1 : 0));
                // Console.WriteLine(working.Aggregate("", (a, b) => a+(b.value*decrypt)+", "));
            }
            Console.WriteLine(working.Aggregate("", (a, b) => a+(b.value*decrypt)+", "));
        }

        var offset = working.FindIndex(p => p.value == 0);

        var result = working[(offset + 1000) % working.Count].value +
            working[(offset + 2000) % working.Count].value +
            working[(offset + 3000) % working.Count].value;

        Console.WriteLine( result * decrypt );
    }
}