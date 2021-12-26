using NUnit.Framework;
using System.Threading.Channels;
namespace AoC2019;

class Day11 : Puzzle
{

    public class HullPainter
    {
        public Dictionary<Point2D, int> hullPainted = new Dictionary<Point2D, int>();
        public Point2D point = Point2D.zero;
        public Facing facing = Facing.Up;
        public int defaultPaint = 0;

        public IEnumerable<long> Output()
        {
            while (true)
            {
                int value = defaultPaint;
                hullPainted.TryGetValue(point, out value);
                // Console.WriteLine("HULLPAINTER: Input sent " + value);
                yield return (long)value;
            }
        }

        public bool nextIsPaint = true;

        public void Input(long value)
        {
            if (nextIsPaint)
            {
                nextIsPaint = false;
                // Console.WriteLine("HULLPAINTER: Paint: " + value);
                hullPainted[point] = (int)value;
            }
            else
            {
                nextIsPaint = true;
                // Console.WriteLine("HULLPAINTER: Turn: " + value);
                if (value == 1)
                {
                    facing = RightTurn[facing];
                }
                else
                {
                    facing = LeftTurn[facing];
                }
                point = point + FacingVector[facing];
            }
        }
    }

    public enum Facing
    {
        Up, Right, Down, Left
    };

    public static Dictionary<Facing, Facing> RightTurn = new Dictionary<Facing, Facing>
    {
        {Facing.Up, Facing.Right},
        {Facing.Right, Facing.Down},
        {Facing.Down, Facing.Left},
        {Facing.Left, Facing.Up}
    };
    public static Dictionary<Facing, Facing> LeftTurn = new Dictionary<Facing, Facing>
    {
        {Facing.Up, Facing.Left},
        {Facing.Left, Facing.Down},
        {Facing.Down, Facing.Right},
        {Facing.Right, Facing.Up}
    };

    public static Dictionary<Facing, Point2D> FacingVector = new Dictionary<Facing, Point2D>
    {
        { Facing.Up, new Point2D(0,-1) },
        { Facing.Down, new Point2D(0,1) },
        { Facing.Left, new Point2D(-1,0) },
        { Facing.Right, new Point2D(1,0) },
    };

    // public override void Part1()
    // {
    //     var robot = new HullPainter();
    //     var computer = new IntcodeComputer(lines[0])
    //     {
    //         inputs = robot.Output().GetEnumerator(),
    //         output = robot.Input
    //     };
    //     computer.RunProgram();

    //     Console.WriteLine("Painted count:" + robot.hullPainted.Count());
    //     // PrintGrid(robot.hullPainted);
    // }

    public static void PrintGrid(Dictionary<Point2D, int> grid, int defaultValue = 0)
    {
        var min = grid.Keys.Aggregate(Point2D.Min);
        var max = grid.Keys.Aggregate(Point2D.Max);

        for (int y = min.y; y <= max.y; y++)
        {
            Console.Write($"{y:00}:");
            for (int x = min.x; x <= max.x; x++)
            {
                int value = defaultValue;
                grid.TryGetValue(new Point2D(x, y), out value);
                Console.Write( value == 0 ? ' ' : '#');
            }
            Console.WriteLine();
        }
    }

    public override void Part2()
    {
        var robot = new HullPainter{defaultPaint = 0};
        robot.hullPainted[Point2D.zero] = 1;
        var computer = new IntcodeComputer(lines[0])
        {
            inputs = robot.Output().GetEnumerator(),
            output = robot.Input,
            // debugger = true,
        };
        computer.RunProgram();

        Console.WriteLine("Painted count:" + robot.hullPainted.Count(v => v.Value == 1));
        PrintGrid(robot.hullPainted);
    }
}