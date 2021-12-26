using NUnit.Framework;
namespace AoC2019;

class Day3 : Puzzle
{
    public List<(Point2D point, int wire, int delay)> grid = new List<(Point2D point, int wire, int delay)>();

    public void ParseLine(string line, int wireNumber)
    {
        int x = 0;
        int y = 0;
        int delay = 0;
        foreach (var move in line.Split(','))
        {
            int distance = Convert.ToInt32(move.Substring(1));
            for (int index = 0; index < distance; index++)
            {
                delay++;
                switch (move[0])
                {
                    case 'R': x++; break;
                    case 'L': x--; break;
                    case 'U': y++; break;
                    case 'D': y--; break;
                    default: throw new Exception("bad move");
                }
                grid.Add((new Point2D(x, y), wireNumber, delay));
            }
        }
    }

    public override void Part1()
    {
        ParseLine(lines[0], 1);
        ParseLine(lines[1], 2);

        var pointCounts = grid.GroupBy(s => s.point).Where(s => s.Key != Point2D.zero && s.Select(s => s.wire).Distinct().Count() > 1);

        Console.WriteLine(pointCounts.Min(s => s.Key.Manhattan()));
        // 462 was wrong
    }

    public override void Part2()
    {
        var pointCounts = grid.GroupBy(s => s.point).Where(s => s.Key != Point2D.zero && s.Select(s => s.wire).Distinct().Count() > 1);
        foreach (var spot in pointCounts)
        {
            foreach (var entry in spot)
            {
                Console.WriteLine("Point: " + entry.point + " Wire: " + entry.wire + " Delay: " + entry.delay);
            }
        }

        Console.WriteLine(pointCounts.Min(s=>s.Sum(w => w.delay)));
        // 462 was wrong

    }
}