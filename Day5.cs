using NUnit.Framework;
namespace AoC2019;

class Day5 : Puzzle
{
    public override void Part1()
    {
        void CheckResult(long result)
        {
            Console.WriteLine("Output: " + result);
        }
        var computer = new IntcodeComputer(lines[0])
        {
            inputs = (new List<long> {1}).GetEnumerator(),
            output = CheckResult
        };
        computer.RunProgram();
    }

    public override void Part2()
    {
        void CheckResult(long result)
        {
            Console.WriteLine("Output: " + result);
        }
        var computer = new IntcodeComputer(lines[0])
        {
            inputs = (new List<long> {5}).GetEnumerator(),
            output = CheckResult
        };
        computer.RunProgram();
    }
}