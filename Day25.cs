using NUnit.Framework;
namespace AoC2019;

class Day25 : Puzzle
{
    public void Input(long i)
    {
        if (i>255 || i < 0) Console.WriteLine(i);
        else
        {
            Console.Write((char)i);
        }
    }

    public IEnumerable<long> Output()
    {
        while (true)
        {
            string? input = Console.ReadLine();
            if (input == null) throw new Exception("Ran out of input");
            for (int x = 0; x < input.Length; x++) yield return input[x];
            yield return 10;
        }
    }

    public override void Part1()
    {
        var computer = new IntcodeComputer(lines[0])
        {
            output = Input,
            inputs = Output().GetEnumerator()
        };
        computer.RunProgram();
    }

    // public override void Part2()
    // {
    // }
}