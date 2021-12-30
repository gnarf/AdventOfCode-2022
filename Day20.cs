using NUnit.Framework;
namespace AoC2019;

class Day20 : Puzzle
{
    public List<Point2D> walls = new List<Point2D>();
    public Dictionary<Point2D, Point2D> warps = new Dictionary<Point2D, Point2D>();
    public Point2D start = Point2D.zero;
    public Point2D end = Point2D.zero;
    public override void Parse(string filename)
    {
        base.Parse(filename);
        ParseLines();
    }

    public void ParseLines()
    {
        walls.Clear();
        List<Point2D> portalPoints = new List<Point2D>();
        portalPoints.Clear();
        for (int y = 0; y < lines.Length; y++)
        {
            var line = lines[y];
            for (int x = 0; x < line.Length; x++)
            {
                if (line[x] == '#')
                {
                    walls.Add(new Point2D(x, y));
                }
                else if (char.IsUpper(line[x]))
                {
                    portalPoints.Add(new Point2D(x, y));
                }
            }
        }

        List<(string code, Point2D point)> portals = new List<(string code, Point2D point)>();

        while (portalPoints.Count > 0)
        {
            var l1 = portalPoints[0];
            portalPoints.RemoveAt(0);
            var l2 = portalPoints.Find(p => p.Manhattan(l1) == 1);
            if (l2 is null)
            {
                Console.WriteLine("Something went wrong, we didn't find our letter pair for this portal: " + l1);
                continue;
            }
            portalPoints.Remove(l2);

            if (l1.x > l2.x || l1.y > l2.y) (l1, l2) = (l2, l1);
            string code = "" + lines[l1.y][l1.x] + lines[l2.y][l2.x];
            var l3 = l2 + (l2 - l1);
            if (l3.y >= lines.Length || l3.x >= lines[l3.y].Length || lines[l3.y][l3.x] != '.')
            {
                l3 = l1 + (l1 - l2);
            }
            // Console.WriteLine($"{code}: {l1} {l2} {l3}");
            // block off the wall code for pathfinder
            walls.Add(l2);
            walls.Add(l1);
            portals.Add((code, l3));
        }

        var pairs = portals.Select(p =>
        {
            return (p, portals.Except(new[] { p }).Where(c => c.code == p.code).FirstOrDefault());
        }).ToList();

        foreach (var (p1, p2) in pairs)
        {
            if (p1.code == "AA") start = p1.point;
            else if (p1.code == "ZZ") end = p1.point;
            else
            {
                warps[p1.point] = p2.point;
                warps[p2.point] = p1.point;
            }
        }
    }

    public override void Part1()
    {
        // Find the shortest path to walk the maze...
        List<(Point2D point, int cost)> stack = new List<(Point2D point, int cost)>
        {
            (start, 0),
        };

        HashSet<Point2D> seen = new HashSet<Point2D>();

        while (stack.Count > 0)
        {
            var (point, cost) = stack[0];
            stack.RemoveAt(0);
            if (seen.Contains(point)) continue;
            seen.Add(point);
            if (point == end)
            {
                Console.WriteLine("Found a winner! " + cost);
                return;
            }

            Point2D? warpDest = Point2D.zero;
            if (warps.TryGetValue(point, out warpDest))
            {
                stack.Add((warpDest, cost + 1));
            }

            foreach (var move in Day11.FacingVector.Values.Select(v => v + point).Where(v => !walls.Concat(seen).Contains(v)))
            {
                stack.Add((move, cost + 1));
            }
        }
    }

    public bool outer(Point2D point)
    {
        Point2D outside = walls.Aggregate(Point2D.Max) - point;
        return point.x < 3 || point.y < 3 || outside.x < 3 || outside.y < 3;
    }

    public override void Part2()
    {
        // Find the shortest path to walk the maze...
        List<(Point2D point, int depth, int cost)> stack = new List<(Point2D point, int depth, int cost)>
        {
            (start, 0, 0),
        };

        HashSet<(Point2D, int depth)> seen = new HashSet<(Point2D, int depth)>();

        Point2D maxWall = walls.Aggregate(Point2D.Max);

        List<(Point2D start, Point2D end, int cost)> routes = new List<(Point2D start, Point2D end, int cost)>();

        void FindRoutes(Point2D start, Point2D[] ends, List<Point2D> walls)
        {
            if (ends.Length == 0) return;
            List<(Point2D point, int cost)> rStack = new List<(Point2D point, int cost)>{(start, 0)};
            HashSet<Point2D> rSeen = new HashSet<Point2D>();
            int found = 0;
            while (rStack.Count > 0)
            {
                var (point, cost) = rStack[0];
                rStack.RemoveAt(0);
                if (rSeen.Contains(point)) continue;
                rSeen.Add(point);
                if (ends.Contains(point))
                {
                    lock(routes){
                        routes.Add((start, point, cost));
                    }
                    if (++found >= ends.Length) return;
                }
                rStack.AddRange(
                    Day11.FacingVector.Values.Select(v=>v+point)
                        .Where(v => !walls.Concat(rSeen).Contains(v))
                        .Select(v => (v, cost + 1))
                );
            }
        }

        var pois = warps.Keys.Append(start).Append(end).ToList();
        for(int x=0; x<pois.Count; x++)
        {
            FindRoutes(pois[x], pois.Skip(x+1).ToArray(), walls);
        }
        routes.AddRange(routes.Select(r => (r.end, r.start, r.cost)).ToArray());
        
        while (stack.Count > 0)
        {
            var (point, depth, cost) = stack[0];
            stack.RemoveAt(0);
            if (seen.Contains((point, depth))) continue;
            seen.Add((point, depth));
            if (point == end && depth == 0)
            {
                Console.WriteLine("Found a winner! " + cost);
                return;
            }

            Point2D? warpDest = Point2D.zero;
            if (warps.TryGetValue(point, out warpDest))
            {
                var outer = (maxWall - point);
                if (point.x < 4 || point.y < 4 || outer.x < 4 || outer.y < 4)
                {
                    if (depth != 0)
                    {
                       stack.Add((warpDest, depth-1, cost + 1));
                    }
                }
                else
                {
                    stack.Add((warpDest, depth+1, cost + 1));
                }
            }

            foreach (var move in routes.Where(v => v.start == point))
            {
                stack.Add((move.end, depth, cost + move.cost));
            }

            stack.Sort((a,b)=>a.cost - b.cost);
        }
    }
}