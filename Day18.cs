using NUnit.Framework;
namespace AoC2019;

class Day18 : Puzzle
{
    public List<Point2D> walls = new List<Point2D>();
    public Dictionary<char, Point2D> locations = new Dictionary<char, Point2D>();
    public static Dictionary<Point2D, char> points = new Dictionary<Point2D, char>();

    public override void Parse(string filename)
    {
        base.Parse(filename);
        ParseLines();
    }

    public void ParseLines()
    {
        walls.Clear();
        locations.Clear();
        for (int y = 0; y < lines.Length; y++)
        {
            var line = lines[y];
            for (int x = 0; x < line.Length; x++)
            {
                if (line[x] == '#')
                {
                    walls.Add(new Point2D(x, y));
                }
                else if (line[x] != '.')
                {
                    locations[line[x]] = new Point2D(x, y);
                    points[new Point2D(x, y)] = line[x];
                }
            }
        }
    }

    public struct Path
    {
        public char from;
        public char to;
        public bool hasPoint(char p) => p == from || p == to;
        public char otherPoint(char p) => p == from ? to : from;
        public int cost;
        public char[] neededKeys;
        public char[] via;

        public override string ToString()
        {
            var neededKeysStr = neededKeys.Aggregate("", (a, b) => a + b);
            var viaStr = via.Aggregate("", (a, b) => a + b);
            return $"Path: {from}->{to} {cost} {neededKeysStr} (via {viaStr})";
        }

        public static Path operator +(Path a, Path b)
        {
            char from;
            char to;
            char extra;
            if (a.from == b.to)
            {
                from = b.from;
                to = a.to;
                extra = a.from;
            }
            else if (b.from == a.to)
            {
                from = a.from;
                to = b.to;
                extra = b.from;
            }
            else if (a.from == b.from)
            {
                from = a.to;
                to = b.to;
                extra = a.from;
            }
            else if (a.to == b.to)
            {
                from = a.from;
                to = b.from;
                extra = a.to;
            }
            else
            {
                throw new Exception("paths don't converge");
            }
            return new Path { from = from, to = to, via = a.via.Append(extra).Union(b.via).ToArray(), neededKeys = a.neededKeys.Union(b.neededKeys).ToArray(), cost = a.cost + b.cost };
        }
    }

    public List<Path> FindPaths(Point2D from, Point2D[] to)
    {
        List<(Point2D pos, int cost, char[] neededKeys, char[] via)> search = new List<(Point2D pos, int cost, char[] neededKeys, char[] via)>
        {
            (from, 0, new char[]{}, new char[] {})
        };

        List<Path> results = new List<Path>();
        if (to.Length == 0) return results;

        HashSet<Point2D> seen = new HashSet<Point2D>();

        while (search.Count > 0)
        {
            (Point2D pos, int cost, char[] neededKeys, char[] via) = search[0];
            search.RemoveAt(0);
            if (walls.Contains(pos)) continue;
            if (seen.Contains(pos)) continue;
            seen.Add(pos);

            if (to.Contains(pos))
            {
                results.Add(new Path { from = points[from], to = points[pos], neededKeys = neededKeys, cost = cost, via = via });
                if (results.Count == to.Length) return results;
            }

            char special = (char)0;
            if (pos != from && points.TryGetValue(pos, out special))
            {
                if (char.IsUpper(special))
                {
                    neededKeys = neededKeys.Append(char.ToLower(special)).OrderBy(a => a).ToArray();
                }
                else if (char.IsLower(special))
                {
                    via = via.Append(points[pos]).ToArray();
                    // Console.WriteLine("Via: " + via.Aggregate("", (a, b) => a + b));
                }
            }

            var next = Day11.FacingVector.Values.Select(v => pos + v).Where(v => !seen.Concat(walls).Contains(v));
            search.AddRange(next.Select(point => (pos: point, cost: cost + 1, neededKeys, via)));
        }
        return results;
    }

    public override void Part1()
    {
        RunSearch();
    }

    public override void Part2()
    {
        Point2D c = locations['@'];
        locations['0'] = c + new Point2D(-1, -1);
        locations['1'] = c + new Point2D(-1, 1);
        locations['2'] = c + new Point2D(1, 1);
        locations['3'] = c + new Point2D(1, -1);
        points[locations['0']] = '0';
        points[locations['1']] = '1';
        points[locations['2']] = '2';
        points[locations['3']] = '3';
        walls.Add(c + new Point2D(1, 0));
        walls.Add(c + new Point2D(-1, 0));
        walls.Add(c + new Point2D(0, 1));
        walls.Add(c + new Point2D(0, -1));
        walls.Add(c);
        points.Remove(locations['@']);
        locations.Remove('@');
        RunSearch();
    }

    public void RunSearch()
    {

        List<char> pois = locations.Keys.Where(c => char.IsLower(c) || char.IsDigit(c) || c == '@').ToList();
        var allPaths = pois.SelectMany((f, i) =>
        {
            Console.WriteLine($"Find Paths: {i} {f}");
            return FindPaths(locations[f], pois.Skip(i + 1).Select(p => locations[p]).ToArray());
        }).ToList();

        Console.WriteLine("All Basic Paths Calculated");

        allPaths = allPaths.SelectMany((f, i) =>
        {
            return allPaths.Skip(i+1).Where(p => (p.hasPoint(f.to)) && !p.hasPoint(f.from) && !p.via.Contains(f.from))
                .Select(p => p + f).Append(f);
        }).ToList();
        allPaths = allPaths.Where((p, i) =>
        {
            if (allPaths.Skip(i + 1).Any(p2 =>
              {
                  return p.hasPoint(p2.from) && p.hasPoint(p2.to) && p.neededKeys.Union(p2.neededKeys).Count() == p2.neededKeys.Length &&
                      (p2.cost < p.cost || (p2.cost == p.cost && p2.via.Length >= p.via.Length));
              }))
            {
                // Console.WriteLine("Culling path: " + p);
                return false;
            }
            return true;
        }).ToList();
        Console.WriteLine("All twostep Paths Calculated");

        char[] start = pois.Where(c => !char.IsLower(c)).ToArray();
        List<(char[] collected, char[] current, int cost)> stack = new List<(char[] collected, char[] current, int cost)>
        {
            (start, start, 0)
        };
        // Dictionary<string, int> seen = new Dictionary<string, int>();

        int cheapestRoute = Int32.MaxValue;
        int seenUse = 0;
        Dictionary<string, int> seenPath = new Dictionary<string, int>();
        while (stack.Count > 0)
        {
            (char[] collected, char[] current, int cost) = stack[^1];
            stack.RemoveAt(stack.Count - 1);

            if (cost >= cheapestRoute) continue;
            var key = collected.Concat(current).Aggregate("", (a, b) => a + b);
            if (seenPath.ContainsKey(key) && seenPath[key] <= cost)
            {
                seenUse++; continue;
            }
            seenPath[key] = cost;

            if (collected.Length == pois.Count)
            {
                Console.WriteLine("Found a new winner! " + cost);
                cheapestRoute = cost;
                continue;
            }

            var toVisit = allPaths.Where(path =>
                cost + path.cost < cheapestRoute &&
                current.Any(path.hasPoint) &&
                path.neededKeys.All(collected.Contains) &&
                !collected.Contains(path.otherPoint(current.Where(path.hasPoint).First()))
            ).Select(path => (
                collected: collected.Union(path.via).Append(path.otherPoint(current.Where(path.hasPoint).First())).OrderBy(a => a).ToArray(),
                current: current.Select(c=>path.hasPoint(c)?path.otherPoint(c):c).ToArray(),
                cost: cost + path.cost
            ));
            stack.AddRange(toVisit);


            stack.Sort((a, b) =>
            {
                var moreSolved = a.collected.Length - b.collected.Length;
                if (moreSolved != 0) return moreSolved;
                return b.cost - a.cost;
            });
        }
    }   
}