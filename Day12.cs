using NUnit.Framework;
using System.Text.RegularExpressions;
namespace AoC2019;

class Day12 : Puzzle
{

    List<Point3D> MoonLocations = new List<Point3D>();
    List<Point3D> MoonVelocities = new List<Point3D>();

    public override void Parse(string filename)
    {
        base.Parse(filename);
        ParseLines();
    }

    public void ParseLines()
    {
        MoonLocations.Clear();
        MoonVelocities.Clear();
        foreach (var line in lines)
        {
            string pattern = @"x=([0-9-]+), y=([0-9-]+), z=([0-9-]+)";
            var match = Regex.Match(line, pattern);
            if (match is not null)
            {
                var coords = match.Groups.Values.Skip(1).Select(g => Convert.ToInt32(g.Value)).ToArray();
                MoonLocations.Add(new Point3D(coords[0], coords[1], coords[2]));
                MoonVelocities.Add(Point3D.zero);
            }
        }
    }

    public long CalculateRepeatCycle(int axis)
    {
        int part(Point3D point)
        {
            if (axis == 0)
            {
                return point.x;
            }
            if (axis == 1)
            {
                return point.y;
            }
            return point.z;
        }

        long cycle = 0;
        int[] initialPositions = MoonLocations.Select(part).ToArray();
        int numMoons = initialPositions.Length;
        int[] initialVelocities = new int[numMoons];
        int[] velocities = new int[numMoons];
        int[] locations = initialPositions.ToArray();
        while(true)
        {
            for (int left=0; left < locations.Count(); left++)
            {
                int lv = velocities[left];
                for (int right = left + 1; right < locations.Count(); right++)
                {
                    int delta = locations[left] - locations[right];
                    int sign = Math.Sign(delta);
                    // Console.WriteLine($"Compare {left} {right} {delta} {sign}");
                    velocities[right] = velocities[right] + sign;
                    velocities[left] = velocities[left] - sign;
                }
            }
            for (int index = 0; index < locations.Count(); index++)
            {
                locations[index] = locations[index] + velocities[index];
            }
            cycle++;
            // if (cycle % 1000 == 0) Console.WriteLine(axis);
            if (velocities.SequenceEqual(initialVelocities) && locations.SequenceEqual(initialPositions))
            {
                return cycle;
            }
        }
    }

    public override void Part1()
    {
        for (int steps = 0; steps < 1000; steps++)
        {
            for (int left=0; left < MoonLocations.Count(); left++)
            {
                Point3D lv = MoonVelocities[left];
                for (int right = left + 1; right < MoonLocations.Count(); right++)
                {
                    Point3D delta = MoonLocations[left] - MoonLocations[right];
                    Point3D sign = Point3D.Sign(delta);
                    // Console.WriteLine($"Compare {left} {right} {delta} {sign}");
                    MoonVelocities[right] = MoonVelocities[right] + sign;
                    MoonVelocities[left] = MoonVelocities[left] - sign;
                }
            }
            for (int index = 0; index < MoonLocations.Count(); index++)
            {
                MoonLocations[index] = MoonLocations[index] + MoonVelocities[index];
            }
        }

        int energy = 0;
        for (int index = 0 ; index < MoonLocations.Count(); index++)
        {
            energy += MoonLocations[index].Manhattan() * MoonVelocities[index].Manhattan();
        }
        Console.WriteLine("Total energy: "+energy);
    }
    static long LCM(long[] numbers)
    {
        return numbers.Aggregate(lcm);
    }
    static long lcm(long a, long b)
    {
        return Math.Abs(a * b) / GCD(a, b);
    }
    static long GCD(long a, long b)
    {
        return b == 0 ? a : GCD(b, a % b);
    }
    public override void Part2()
    {
        ParseLines();
        Task<long>[] tasks = new[]
        {
            Task.Run(() => CalculateRepeatCycle(0)),
            Task.Run(() => CalculateRepeatCycle(1)),
            Task.Run(() => CalculateRepeatCycle(2)),
        };
        long x = tasks[0].Result;
        long y = tasks[1].Result;
        long z = tasks[2].Result;
        long lcm = LCM(new[]{x,y,z});
        Console.WriteLine($"{x} {y} {z} -- {lcm}");
    }
}