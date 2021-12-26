using NUnit.Framework;
namespace AoC2019;

class Day10 : Puzzle
{

    List<Point2D> points = new List<Point2D>();

    public override void Parse(string filename)
    {
        base.Parse(filename);
        points.Clear();
        for (int y = 0; y < lines.Length; y++)
        {
            var line = lines[y];
            for (int x = 0; x < line.Length; x++)
            {
                if (line[x] == '#') points.Add(new Point2D(x, y));
            }
        }
    }

    public override void Part1()
    {
        
    }

    // public override void Part2()
    // {
    // }
}