using NUnit.Framework;
namespace AoC2019;

class Day1 : Puzzle
{
    [TestCase(12, ExpectedResult = 2)]
    [TestCase(100756, ExpectedResult = 33583)]
    public int FuelRequired(int mass) => (mass / 3) - 2;

    public int MassOfLines(Func<int, int>massFunc)
    {
        int total = 0;
        foreach (string line in lines)
        {
            int mass = Convert.ToInt32(line);
            total += massFunc(mass);
        }
        return total;
    }

    [TestCase(100756, ExpectedResult = 50346)]
    public int FuelWithFuelCost(int mass)
    {
        int fuelMass = FuelRequired(mass);
        int massToCalc = fuelMass;
        while (massToCalc > 0)
        {
            massToCalc = FuelRequired(massToCalc);
            if (massToCalc > 0)
            {
                fuelMass += massToCalc;
            }
        }
        return fuelMass;
    }

    public override void Part1()
    {
        Console.WriteLine(MassOfLines(FuelRequired));
    }

    public override void Part2()
    {
        var fuel = MassOfLines(FuelWithFuelCost);
        Console.WriteLine(fuel);
    }
}