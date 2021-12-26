using NUnit.Framework;
namespace AoC2019;

class Day2 : Puzzle
{
    public long result;
    public override void Part1()
    {
        var computer = new IntcodeComputer(lines[0]);
        computer[1] = 12;
        computer[2] = 2;
        computer.RunProgram();
        result = computer[0];
        Console.WriteLine(result);
    }

    public override void Part2()
    {
        for(int noun = 0; noun < 100; noun++)
        {
            for(int verb = 0; verb < 100; verb++)
            {
                var computer = new IntcodeComputer(lines[0]);
                computer[1] = noun;
                computer[2] = verb;
                computer.RunProgram();

                if (computer[0] == 19690720)
                {
                    Console.WriteLine(result = 100*noun + verb);
                    return;
                }

            }
        }
    }

    [TestCase(ExpectedResult=3850704)]
    public static long TestMyResults()
    {
        var runner = new Day2();
        runner.Parse("F:/code/AdventOfCode-2019/input/2.txt");
        runner.Part1(); 
        return runner.result;
    }

    [TestCase(ExpectedResult=6718)]
    public static long TestMyResults2()
    {
        var runner = new Day2();
        runner.Parse("F:/code/AdventOfCode-2019/input/2.txt");
        runner.Part2(); 
        return runner.result;
    }
}