using NUnit.Framework;
namespace AoC2022;

class Day1 : Puzzle
{
    List<long> elves = new List<long>{0};

    public override void Part1()
    {

        int y = 0;
        for (int x=0;x<lines.Count();x++)
        {
            if (String.IsNullOrEmpty(lines[x]))
            {
                elves.Add(0);
                ++y;
            }
            else
            {
                elves[y]+= Convert.ToInt32(lines[x]);

            }
        }
        Console.WriteLine(elves.Max());
    }

    public override void Part2()
    {
        Console.WriteLine(elves.ToList().Sorted().Reversed().Take(3).Sum());
    }
}