using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
namespace AoC2022;

class Day21 : Puzzle
{

    public class MathMonkey
    {
        public string name = "";
        public long? initialValue = null;
        public string other1 = "";
        public string other2 = "";
        public string op = "";

        public long value
        {
            get
            {
                if (initialValue != null) return (long)initialValue;
                return op switch
                {
                    "+" => MonkeysByName[other1].value + MonkeysByName[other2].value,
                    "-" => MonkeysByName[other1].value - MonkeysByName[other2].value,
                    "*" => MonkeysByName[other1].value * MonkeysByName[other2].value,
                    "/" => MonkeysByName[other1].value / MonkeysByName[other2].value,
                    _ => throw new Exception()
                };
            }
        }

        public bool usesHuman()
        {
            if (name == "humn") return true;
            if (initialValue != null) return false;
            if (MonkeysByName[other1].usesHuman() || MonkeysByName[other2].usesHuman()) return true;
            return false;
        }
    }

    public static Dictionary<string, MathMonkey> MonkeysByName = new();

    public override void Parse(string filename)
    {
        base.Parse(filename);
        foreach(var line in lines)
        {
            var parts = line.Split(new char[]{':', ' '}, StringSplitOptions.RemoveEmptyEntries);
            var monkey = new MathMonkey { name = parts[0] };
            if (parts.Length == 2)
            {
                monkey.initialValue = int.Parse(parts[1]);
            }
            else
            {
                monkey.other1 = parts[1];
                monkey.op = parts[2];
                monkey.other2 = parts[3];
            }
            MonkeysByName.Add(monkey.name, monkey);
        }
    }

    public override void Part1()
    {
        Console.WriteLine(MonkeysByName["root"].value);
    }

    public override void Part2()
    {
        var root = MonkeysByName["root"];
        
        // figure out which branch has the human and calculate the other.
        Console.WriteLine($"root: {root.other1} == {root.other2}");
        long constantSide;
        MathMonkey humanSide;
        if (MonkeysByName[root.other1].usesHuman())
        {
            humanSide = MonkeysByName[root.other1];
            constantSide = MonkeysByName[root.other2].value;
        }
        else
        {
            humanSide = MonkeysByName[root.other2];
            constantSide = MonkeysByName[root.other1].value;
        }

        Console.WriteLine($"root: {humanSide.name} == {constantSide}");

        long resultReversed = constantSide;

        while (humanSide.name != "humn")
        {
            Console.WriteLine($"{humanSide.name}: {humanSide.other1} {humanSide.op} {humanSide.other2}");

            var nextHumanSide = MonkeysByName[humanSide.other1].usesHuman() ? humanSide.other1 : humanSide.other2;
            var nextMonkeySide = MonkeysByName[humanSide.other1].usesHuman() ? humanSide.other2 : humanSide.other1;

            var otherSide = MonkeysByName[nextMonkeySide].value;

            if (humanSide.op == "*")
            {
                resultReversed /= otherSide;
            }
            else
            if (humanSide.op == "+")
            {
                resultReversed -= otherSide;
            }
            else
            if (humanSide.op == "/")
            {
                if (nextHumanSide == humanSide.other1)
                {
                    // human left side of /
                    resultReversed *= otherSide;
                } else {
                    throw new Exception();
                }
            }
            else
            if (humanSide.op == "-")
            {
                if (nextHumanSide == humanSide.other1)
                {
                    // human left side of -
                    resultReversed += otherSide;
                } else {
                    // result == const - human
                    // human == const - result;
                    resultReversed = otherSide - resultReversed;
                }
            }
            else
            {
                throw new Exception();
            }

            humanSide = MonkeysByName[nextHumanSide];
        }
        Console.WriteLine($"humn: {resultReversed}");
    }
}