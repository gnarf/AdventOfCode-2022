using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
namespace AoC2022;

class Day18 : Puzzle
{

    public HashSet<Point3D> cubes = new();

    public override void Parse(string filename)
    {
        base.Parse(filename);
        foreach (var line in lines)
        {
            int[] parts = line.Split(',').Select(s => int.Parse(s)).ToArray();
            cubes.Add(new Point3D(parts[0], parts[1], parts[2]));
        }
    }

    public override void Part1()
    {
        int surfArea = 0;
        foreach (var cube in cubes)
        {
            foreach (var face in Point3D.FacingToPointVector.Values)
            {
                if (!cubes.Contains(cube + face)) { surfArea++; }
            }
        }
        Console.WriteLine(surfArea);
    }

    public override void Part2()
    {
        int surfArea = 0;
        // we can count the same way if we fill in the "internal" holes.
        var min = Point3D.Abs(cubes.Aggregate(Point3D.Min));
        var max = cubes.Aggregate(Point3D.Max);

        var openAir = new Dictionary<Point3D, bool>();

        var probing = new HashSet<Point3D>();

        bool? isOpenAir(Point3D point)
        {
            if (openAir.TryGetValue(point, out var val)) return val;
            if (point.x < min.x || point.y < min.y || point.z < min.z || point.x > max.x || point.y > max.y || point.z > max.z)
                return true;
            
            if (cubes.Contains(point))
            {
                // openAir[point] = false;
                return false;
            }

            probing.Add(point);

            // if any point is not being probed, not part of a cube, and is open air, we are open air.
            if (Point3D.FacingToPointVector.Values.Any(f => !probing.Contains(point+f) && !cubes.Contains(point + f) && isOpenAir(point + f) == true))
            {
                openAir.Add(point, true);
                return true;
            }

            return null;

        }

        for (int c = 0; c<cubes.Count; c++)
        {
            var cube = cubes.ElementAt(c);
            foreach (var face in Point3D.FacingToPointVector.Values)
            {
                probing.Clear();
                if (isOpenAir(cube + face) == true) surfArea++;
            }
        }

        // 2229 too low
        // 2463 too low
        Console.WriteLine(surfArea);
    }
}