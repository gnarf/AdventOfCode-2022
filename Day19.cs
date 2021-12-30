using NUnit.Framework;
namespace AoC2019;

class Day19 : Puzzle
{

    public class Prober
    {
        public List<Point2D> toExplore = new List<Point2D> { Point2D.zero };
        public Point2D LastPoint = Point2D.zero;
        public Dictionary<Point2D, int> screen = new Dictionary<Point2D, int>();
        public IEnumerable<long> Output()
        {
            while (toExplore.Count > 0)
            {
                Point2D point = toExplore[0];
                toExplore.RemoveAt(0);
                // Console.WriteLine("Exploring: " + point);
                LastPoint = point;
                yield return (long)point.x;
                yield return (long)point.y;
            }
        }

        public void Input(long result)
        {
            screen[LastPoint] = (int)result;
        }

        public void RunCycle()
        {
            var computer = new IntcodeComputer(lines[0])
            {
                inputs = Output().GetEnumerator(),
                output = Input
            };
            computer.RunProgram();
        }

        public int Explore(Point2D point)
        {
            if (screen.ContainsKey(point)) return screen[point];
            toExplore.Insert(0, point);
            RunCycle();
            return screen[point];
        }
    }

    public override void Part1()
    {
        var probe = new Prober();
        for (int x = 0; x < 50; x++) for (int y = 0; y < 50; y++) probe.toExplore.Add(new Point2D(x, y));
        while (probe.toExplore.Count > 0)
        {
            probe.RunCycle();
        }
        Day11.PrintGrid(probe.screen);
        Console.WriteLine("Lit: " + probe.screen.Values.Count(x => x == 1));
    }

    public override void Part2()
    {
        var probe = new Prober();
        for (int x = 0; x < 50; x++) for (int y = 0; y < 50; y++) probe.Explore(new Point2D(x, y));
        Day11.PrintGrid(probe.screen);
        Console.WriteLine("Lit: " + probe.screen.Values.Count(x => x == 1));
        double minTan = 5;
        double maxTan = 0;
        double maxUnder = 0;
        double minOver = 5;
        void parse(Point2D point, int value)
        {
            double tan = point.y / (double)point.x;
            if (value == 1)
            {
                maxTan=Math.Max(tan, maxTan);
                minTan=Math.Min(tan, minTan);
            }
            else
            {
                if (tan<minTan && tan>maxTan) return; // we havent found a safe angle yet
                if (tan<minTan) maxUnder = Math.Max(maxUnder, tan);
                if (tan>minTan) minOver = Math.Min(minOver, tan);
            }
        }
        foreach(var kv in probe.screen)
        {
            if (kv.Key.x == 0 || kv.Key.y == 1) continue;
            parse(kv.Key, kv.Value);
            // Console.WriteLine($"Calculating {kv.Key} {kv.Value} {tan} {minAngle - maxUnder} {minOver - maxAngle}");
        }

        bool explore(Point2D p)
        {
            int result = probe.Explore(p);
            parse(p, result);
            return result == 1;
        }

        int xGuess = (int)Math.Floor(200/(minOver-maxUnder));
        int yGuess = (int)(maxUnder * xGuess);
        yGuess+=100;
        while (true)
        {
            var p = new Point2D(xGuess, yGuess);
            var hit = explore(p);
            if (!hit)
            {
                throw new Exception("expected to hit here...");
            }
            // Console.WriteLine($"Testing {p} {explore(p)}");
            if (!explore(p+new Point2D(99,0)))
            {
                // Console.WriteLine($"Not 100 wide here, move down one row");
                if (!explore(p+new Point2D(0,1)))
                {
                    // Console.WriteLine("Not available below, scoot right");
                    xGuess++;
                    continue;
                }
                yGuess++;
                continue;
            }
            if (!explore(p+new Point2D(0, 99)))
            {
                // Console.WriteLine("Not 100 tall here, move right");
                if (!explore(p + new Point2D(1,0)))
                {
                    // Console.WriteLine("Right not available");
                    yGuess++;
                    continue;
                }
                xGuess++;
                continue;
            }
            if (explore(p+new Point2D(99,99)))
            {
                Console.WriteLine($"Found a 100x100 box at {p}: {p.x*10000 + p.y}");
                return;
            }
        }
    }
}