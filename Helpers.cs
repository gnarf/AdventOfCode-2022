namespace AoC2022;
using System.IO;

public abstract class Puzzle {
    public static string[] lines = new string[0];
    public virtual void Parse(string filename)
    {
        lines = File.ReadAllLines(filename);
    }

    public virtual void Part1() => Console.WriteLine("Not implemented yet");
    public virtual void Part2() => Console.WriteLine("Not implemented yet");
}