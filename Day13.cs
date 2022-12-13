using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace AoC2022;

class Day13 : Puzzle
{
    public class PairListValue : IComparable<PairListValue>
    {
        public int? Value = null;
        public List<PairListValue>? List = null;

        public PairListValue()
        {
            this.List = new();
        }
        public PairListValue(int Value) => this.Value = Value;
        public PairListValue(List<PairListValue> Value)
        {
            this.List = Value;
        }

        public PairListValue? this[int i] => List != null && i < List.Count ? List[i] : null;
        public PairListValue? ItemAt(int i) => this[i];

        public void Add(PairListValue? value)
        {
            if (value == null) throw new Exception("null value add");
            this.List?.Add(value);
        }

        public override string ToString()
        {
            if (List != null)
            {
                return $"[{List.Aggregate("", (a,b) => $"{a}{b},").TrimEnd(',')}]";
            }
            return Value?.ToString() ?? "null";
        }

        public bool isValue => Value != null;
        int v => Value ?? 0;
        public int Length => List?.Count ?? 1;

        public int CompareTo(PairListValue? other)
        {
            if (other == null) throw new Exception("compare to null");
            return CompareToNotNull(other);
        }

        public int CompareToNotNull(PairListValue other)
        {
            if (isValue && other.isValue)
            {
                return v - other.v;
            }
            if (!isValue && !other.isValue)
            {
                if (List == null) throw new Exception("null guard List");
                for (int x=0; x<Length; x++)
                {
                    if (x >= other.Length) return 1;
                    var compare = List[x].CompareTo(other[x]);
                    if (compare != 0) return compare;
                }
                if (other.Length > Length) return -1;
                return 0;
            }
            else
            {
                if (isValue)
                {
                    var tempList = new PairListValue(new List<PairListValue>{this});
                    return tempList.CompareTo(other);
                }
                else
                {
                    var tempList = new PairListValue(new List<PairListValue>{other});
                    return this.CompareTo(tempList);
                }
            }
            throw new NotImplementedException();
        }
    }

    public List<PairListValue> Pairs = new();
    public List<PairListValue> LineValues = new();

    public override void Parse(string filename)
    {
        base.Parse(filename);
        Stack<PairListValue> parsing = new();
        PairListValue? parsingPair = new();
        for (int x=0; x<lines.Length; x++)
        {
            if (x % 3 == 2) continue;
            if (x % 3 == 0) { parsingPair = new PairListValue(); Pairs.Add(parsingPair); parsing.Clear(); }
            var len = 0;
            for (int c=0; c<lines[x].Length; c++)
            {
                var ch = lines[x][c];
                if (ch == '[')
                {
                    var n = new PairListValue();
                    parsingPair?.Add(n);
                    if (parsingPair!=null)
                        parsing.Push(parsingPair);
                    parsingPair = n;
                }
                else if (ch == ']')
                {
                    if (len > 0)
                    {
                        var value = lines[x].Substring(c-len, len);
                        parsingPair?.Add(new PairListValue(int.Parse(value)));
                        len = 0;
                    }
                    parsingPair = parsing.Pop();

                }
                else if (ch != ',')
                {
                    len++;
                }
                else
                {
                    if (len > 0)
                    {
                        var value = lines[x].Substring(c-len, len);
                        parsingPair?.Add(new PairListValue(int.Parse(value)));
                        len = 0;
                    }
                }
            }
            if (x%3 == 1 && parsingPair != null && parsingPair.Length == 2)
            {
                LineValues.Add(parsingPair?.ItemAt(0) ?? throw new Exception("no left?"));
                LineValues.Add(parsingPair?.ItemAt(1) ?? throw new Exception("no right?"));
            }
            Console.WriteLine($"{lines[x]} -> {parsingPair}");
        }
    }

    public override void Part1()
    {
        int sum = 0;
        for (int x=0; x<Pairs.Count; x++)
        {
            var pair = Pairs[x];
            var left = pair[0];
            var right = pair[1];
            if (left == null || right == null) throw new Exception("left right null guard");
            var ok = left.CompareTo(right) <= 0;
            if (ok) sum += x+1;
            Console.WriteLine($"{x+1}: {ok} {left} {right} {left.CompareTo(right)}");

        }
        Console.WriteLine(sum);
    }

    public override void Part2()
    {
        PairListValue divider1 = new PairListValue();
        divider1.Add(new PairListValue());
        divider1.ItemAt(0)?.Add(new PairListValue(2));
        PairListValue divider2 = new PairListValue();
        divider2.Add(new PairListValue());
        divider2.ItemAt(0)?.Add(new PairListValue(6));

        LineValues.Add(divider1);
        LineValues.Add(divider2);

        LineValues.Sort();

        foreach(var line in LineValues)
        {
            Console.WriteLine(line);
        }

        var item1 = LineValues.IndexOf(divider1) + 1;
        var item2 = LineValues.IndexOf(divider2) + 1;

        Console.WriteLine(item1 * item2);
    }
}