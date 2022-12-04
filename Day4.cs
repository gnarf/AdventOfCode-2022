using NUnit.Framework;
namespace AoC2022;

class Day4 : Puzzle
{
    public override void Part1()
    {
        int count = 0;
        foreach (var line in lines)
        {
            var numbers = line.Split(new char[] {',', '-'}).Select(s => Convert.ToInt32(s)).ToArray();
            if (numbers[0] <= numbers[2] && numbers[1] >= numbers[3])
            {
                count++;
            }
            else
            if (numbers[2] <= numbers[0] && numbers[3] >= numbers[1])
            {
                count++;
            }
        }
        Console.WriteLine(count);
    }

    public override void Part2()
    {
        int count = 0;
        foreach (var line in lines)
        {
            var numbers = line.Split(new char[] {',', '-'}).Select(s => Convert.ToInt32(s)).ToArray();
            if (numbers[0] <= numbers[2] && numbers[1] >= numbers[3])
            {
                count++;
            }
            else
            if (numbers[2] <= numbers[0] && numbers[3] >= numbers[1])
            {
                count++;
            }
            else
            if (numbers[2] >= numbers[0] && numbers[2] <= numbers[1])
            {
                count++;
            }
            else
            if (numbers[3] >= numbers[0] && numbers[3] <= numbers[1])
            {
                count++;
            }
            else
            if (numbers[0] >= numbers[2] && numbers[0] <= numbers[3])
            {
                count++;
            }
            else
            if (numbers[1] >= numbers[2] && numbers[1] <= numbers[3])
            {
                count++;
            }
        }
        Console.WriteLine(count);

    }
}