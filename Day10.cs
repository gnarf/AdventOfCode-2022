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

    Point2D? maxVisionPoint;


    public override void Part1()
    {
        var sees = points.Select(point =>
        {
            // Console.WriteLine("Parse -- " + point);
            var angles = points.Where(p => p != point).Select(p2 =>
            {
                var delta = p2 - point;
                var angle = Math.Atan2(delta.y, delta.x);
                // Console.WriteLine(String.Format("{0} - {1} -> angle {2}", point, p2, angle));
                return angle;
            }).Distinct().ToList();
            // Console.WriteLine("Distinct angles " + angles.Count());
            return angles.Count();
        }).ToList();
        var maxCount = sees.Max();
        Console.WriteLine("Maximum detection: " + maxCount);
        maxVisionPoint = points[sees.FindIndex(s => s == maxCount)];
    }

    public override void Part2()
    {
        if (maxVisionPoint is null) throw new Exception("Run part1 first for this one");
        var pointsMap = points.Where(p => p != maxVisionPoint).Select(p =>
        {
            var delta = p - maxVisionPoint;
            return (point: p, angle: ((Math.Atan2(delta.y, delta.x) * 180 / Math.PI)+90+360) % 360, dist: delta.Manhattan());
        }).ToList();
        pointsMap.Sort((a, b) =>
        {
            if (a.angle < b.angle) return -1;
            if (a.angle > b.angle) return 1;
            if (a.dist < b.dist) return -1;
            if (a.dist > b.dist) return 1;
            return 0;
        });

        double lastAngleDestroyed = 0;
        int destroyCount = 1;
        Point2D winner = Point2D.zero;
        pointsMap.RemoveAt(0);
        List<int> destroyed = new List<int>();

        while (pointsMap.Count > 0)
        {
            for(int x=0; x<pointsMap.Count; x++)
            {
                var (point, angle, dist) = pointsMap[x];
                if (angle != lastAngleDestroyed)
                {
                    destroyed.Add(x);
                    destroyCount++;
                    // Console.WriteLine(String.Format("BOOM! {0}, {1}", destroyCount, point));
                    if (destroyCount == 200)
                    {
                        Console.WriteLine("Lucky winner is " + point);
                        winner = point;
                    }
                    lastAngleDestroyed = angle;
                }
            }
            lastAngleDestroyed = -1;
            destroyed.Reverse();
            foreach(var point in destroyed)
            {
                pointsMap.RemoveAt(point);
            }
            destroyed.Clear();
        }
    }
}