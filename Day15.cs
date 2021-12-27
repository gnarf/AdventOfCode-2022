using NUnit.Framework;
namespace AoC2019;

class Day15 : Puzzle
{
    public static Dictionary<Facing2D, Point2D> FacingVector = new Dictionary<Facing2D, Point2D>
    {
        { Facing2D.Down, new Point2D(0,1) },
        { Facing2D.Up, new Point2D(0,-1) },
        { Facing2D.Left, new Point2D(-1,0) },
        { Facing2D.Right, new Point2D(1,0) },
    };

    public class RepairDroid
    {
        public Point2D position = Point2D.zero;
        public Point2D? targetLocation;

        public List<Facing2D> walkBack = new List<Facing2D>();

        public HashSet<Point2D> walls = new HashSet<Point2D>();
        public HashSet<Point2D> visited = new HashSet<Point2D>
        {
            Point2D.zero
        };
        public Facing2D lastCommand = Facing2D.Up;

        public static Dictionary<Facing2D, Facing2D> AboutFace = new Dictionary<Facing2D, Facing2D>
        {
            {Facing2D.Up, Facing2D.Down},
            {Facing2D.Down, Facing2D.Up},
            {Facing2D.Left, Facing2D.Right},
            {Facing2D.Right, Facing2D.Left}
        };

        public static Dictionary<Facing2D, long> FacingCommand = new Dictionary<Facing2D, long>
        {
            { Facing2D.Up, 1 },
            { Facing2D.Down, 2},
            { Facing2D.Left, 3},
            { Facing2D.Right, 4},
        };

        public bool backWalking = false;
        public bool debugging = false;

        public IEnumerable<long> Output()
        {
            while (true)
            {
                if (debugging)
                {
                    // Console.Clear();
                    Console.WriteLine();
                    PrintGrid();
                }
                // what direction do we want to move...
                bool found = backWalking = false;
                foreach ((Facing2D f, Point2D v) in FacingVector)
                {
                    if (!walls.Concat(visited).Contains(v+position))
                    {
                        lastCommand = f;
                        found = true;
                    }
                }
                if (!found)
                {
                    if (walkBack.Count > 0)
                    {
                        backWalking = true;
                        lastCommand = walkBack[^1];
                        walkBack.RemoveAt(walkBack.Count - 1);
                    }
                    else
                    {
                        yield return -1;
                        throw new Exception("Done");
                    }
                }
                yield return FacingCommand[lastCommand];
            }
        }

        public void Input(long result)
        {
            if (result == 0)
            {
                // wall we didnt move
                walls.Add(position + FacingVector[lastCommand]);
            }
            else if (result == 1)
            {
                // empty space, we moved
                position = position + FacingVector[lastCommand];
                visited.Add(position);
                if (!backWalking) walkBack.Add(AboutFace[lastCommand]);
            }
            else
            {
                // this is the target!
                position = position + FacingVector[lastCommand];
                targetLocation = position;
                visited.Add(position);
                if (!backWalking) walkBack.Add(AboutFace[lastCommand]);
            }
        }

        public void PrintGrid()
        {
            var min = walls.Prepend(position).Aggregate(Point2D.Min);
            var max = walls.Prepend(position).Aggregate(Point2D.Max);

            for (int y = min.y; y <= max.y; y++)
            {
                Console.Write($"{y:+00;-00}:");
                for (int x = min.x; x <= max.x; x++)
                {
                    if (walls.Contains(new Point2D(x, y)))
                    {
                        Console.Write('#');
                    }
                    else if (targetLocation is not null && targetLocation.x == x && targetLocation.y == y)
                    {
                        Console.Write('x');
                    }
                    else if (position.x == x && position.y == y)
                    {
                        Console.Write('+');
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
                Console.WriteLine();
            }
        }
    }

    public override void Part1()
    {
        var robot = new RepairDroid();
        var computer = new IntcodeComputer(lines[0])
        {
            inputs = robot.Output().GetEnumerator(),
            output = robot.Input
        };
        computer.RunProgram();
        robot.PrintGrid();
        if (robot.targetLocation is null) throw new Exception("Didn't find target");

        List<(Point2D point, int cost)> field = new List<(Point2D point, int cost)>(FacingVector.Values.Intersect(robot.visited).Select(point=>(point,cost: 1)));
        Dictionary<Point2D, int> costs = new Dictionary<Point2D, int>
        {
            {Point2D.zero, 0}
        };

        while (field.Count > 0)
        {
            (Point2D current, int cost) = field[0];
            field.RemoveAt(0);
            int oldCost = Int32.MaxValue;
            if (!costs.TryGetValue(current, out oldCost) || oldCost > cost)
            {
                costs[current] = cost;
                field.AddRange(FacingVector.Values.Select(v => v+current).Intersect(robot.visited).Select(point => (point, cost: cost+1)));
            }
        }
        Console.WriteLine("Cheapest route to x: " + costs[robot.targetLocation]);
    }

    public override void Part2()
    {
        var robot = new RepairDroid();
        var computer = new IntcodeComputer(lines[0])
        {
            inputs = robot.Output().GetEnumerator(),
            output = robot.Input
        };
        computer.RunProgram();
        robot.PrintGrid();

        if (robot.targetLocation is null) throw new Exception("Didn't find target");

        List<(Point2D point, int cost)> field = new List<(Point2D point, int cost)>(FacingVector.Values.Select(p=>p+robot.targetLocation).Intersect(robot.visited).Select(point=>(point,cost: 1)));
        Dictionary<Point2D, int> costs = new Dictionary<Point2D, int>
        {
            {Point2D.zero, 0}
        };

        while (field.Count > 0)
        {
            (Point2D current, int cost) = field[0];
            field.RemoveAt(0);
            int oldCost = Int32.MaxValue;
            if (!costs.TryGetValue(current, out oldCost) || oldCost > cost)
            {
                costs[current] = cost;
                field.AddRange(FacingVector.Values.Select(v => v+current).Intersect(robot.visited).Select(point => (point, cost: cost+1)));
            }
        }
        Console.WriteLine("Max route from x: " + costs.Values.Max());

    }
}