using NUnit.Framework;
using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
namespace AoC2022;

class Day15 : Puzzle
{

    Regex notNumbers = new Regex("[^-0-9]+", RegexOptions.Compiled);

    List<Point2D> knownBeacons = new();
    Dictionary<Point2D, int> sensorReadings = new Dictionary<Point2D, int>();

    public override void Parse(string filename)
    {
        base.Parse(filename);
        foreach (var line in lines)
        {
            var strs = notNumbers.Split(line);
            var parts = strs.Where(s => !String.IsNullOrEmpty(s)).Select(int.Parse).ToArray();
            var sensor = new Point2D(parts[0], parts[1]);
            var beacon = new Point2D(parts[2], parts[3]);
            knownBeacons.Add(beacon);
            sensorReadings.Add(sensor, (int)(beacon - sensor).Manhattan());
        }
    }

    public override void Part1()
    {
        long min = sensorReadings.Keys.Aggregate(Point2D.Min).x - sensorReadings.Values.Max();
        long max = sensorReadings.Keys.Aggregate(Point2D.Max).x + sensorReadings.Values.Max();
        int count = 0;
        for (long x=min; x<=max; x++)
        {
            var p = new Point2D(x, 2000000);
            if (knownBeacons.Contains(p))
            {
                // Console.WriteLine($"Beacon at {p}");
            }
            else
            if (sensorReadings.Keys.Contains(p))
            {
                // Console.WriteLine($"Sensor at {p}");
            }
            else
            if (sensorReadings.Any(r => (r.Key - p).Manhattan() <= r.Value))
            {
                // Console.WriteLine($"Hit at {p}");
                count++;
            }
            else
            {
                // Console.WriteLine($"Miss at {p}");
            }
        }
        Console.WriteLine($"Hits {count}");
        Console.WriteLine();
    }


/*

Mathy notes while the brute method runs:

We effictively are searching for a solution to : |x-sX| + |y-sY| > sD (sensor xy, distance)

we can't do absolute value but we can square and square root like silly people with hard math problems

sqrt( (x - sx) ^ 2 ) + sqrt( (y - sy ) ^ 2) > sd

but this feels harder

how else to write one sensor's exclusions

for instance: at 0, 0 with a distance detected of 5

Solutions: 
    x<-5
    -5 <= x <= 0, y < -x - 5 (or) y > x + 5
    0 < x <= 5, y < x - 5 (or) y > 5 - x
    x > 5

for instance: at 10, 10 with a distance detected of 5

Solutions: 
    x < 5 (10-5)
    5 <= x <= 10, y < 15 - x (or) y > x + 5
    10 < x <= 15, y < x - 5 (or) y > 25 - x
    x > 15

attempt at a system via WA:


Input
{abs(x - 4) + abs(y - 4)>5, abs(x) + abs(y)>5}
Alternate form assuming x and y are real
{sqrt((x - 4)^2) + sqrt((y - 4)^2)>5, sqrt(x^2) + sqrt(y^2)>5}
Real solutions
x<-5
-5<=x<=0, y<-x - 5
-5<=x<=0, y>x + 5
0<x<=4, y<x - 5
0<x<=4, y>x + 5
4<x<=9, y<x - 5
4<x<=9, y>13 - x
x>9




*/

    public override void Part2()
    {
        // foreach (var sensor in sensorReadings)
        // {
        //     Console.WriteLine($" |{sensor.Key.x} - x| + |{sensor.Key.y} - y| > {sensor.Value}");
        // }
        int searchSpace = 4000000;        

        for (int x=0; x<=searchSpace; x++)
        {
            List<(long left, long right)> ranges = sensorReadings
            .Where( r => Math.Abs(r.Key.x - x) <= r.Value )
            .Select(r => {
                var dx = Math.Abs(r.Key.x - x);
                var left = r.Key.y - (r.Value - dx);                
                var right = r.Key.y + (r.Value - dx);                
                return (left, right);
            })
            .ToList()
            .Sorted();


            int rightMost = 0;

            foreach ((var left, var right) in ranges)
            {
                while (left > rightMost + 1)
                {
                    var p = new Point2D(x, rightMost + 1);
                    if (knownBeacons.Contains(p) || sensorReadings.Keys.Contains(p)) rightMost++;
                    else
                    {
                        Console.WriteLine($"win condition {p} {(long)p.x*searchSpace + p.y}");
                        return;
                    }
                }
                rightMost = (int)Math.Max(right, rightMost);
            }
        }
    }
}