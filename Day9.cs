using NUnit.Framework;
namespace AoC2019;

class Day9 : Puzzle
{
    public override void Part1()
    {
        var computer = new IntcodeComputer(lines[0])
        {
            inputs = (new List<long>{1}).GetEnumerator(),
            output = v => Console.WriteLine(v)
        };
        computer.RunProgram();
    }

    public override void Part2()
    {
        var computer = new IntcodeComputer(lines[0])
        {
            inputs = (new List<long>{2}).GetEnumerator(),
            output = v => Console.WriteLine(v)
        };
        computer.RunProgram();
    }
}