using NUnit.Framework;
namespace AoC2019;

class Day13 : Puzzle
{
    public class Arcade
    {
        public Dictionary<Point2D, int> screen = new Dictionary<Point2D, int>();
        public int defaultPaint = 0;

        public int index = 0;
        public int x;
        public int y;
        public int score;

        public Point2D ball = Point2D.zero;
        public Point2D ballv = Point2D.zero;
        public Point2D paddle = Point2D.zero;

        public void Input(long value)
        {
            if (index == 0)
            {
                x = (int)value;
            }
            else if (index == 1)
            {
                y = (int)value;
            }
            else
            {
                if (x == -1 && y == 0) score = (int)value;
                else
                {
                    screen[new Point2D(x, y)] = (int)value;
                    if (value == 4)
                    {
                        var newball = new Point2D(x, y);
                        ballv = Point2D.Sign(newball - ball);
                        ball = newball;
                    }
                    if (value == 3)
                    {
                        paddle = new Point2D(x, y);
                    }
                }
            }
            index = (index + 1) % 3;
        }

        public IEnumerable<long> Output()
        {
            while (true)
            {
                // PrintGrid(screen);
                // Console.WriteLine($"Ball: {ball}({ballv}) Paddle:{paddle} {headingloc} {bounce}");
                if (paddle.x < ball.x) yield return 1;
                else if (paddle.x == ball.x) yield return 0;
                else if (paddle.x > ball.x) yield return -1;
            }
        }
    }

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
                Console.Write(value);
            }
            Console.WriteLine();
        }
    }

    public override void Part1()
    {
        var arcade = new Arcade();
        var computer = new IntcodeComputer(lines[0])
        {
            output = arcade.Input,
            // debugger = true,
        };
        computer.RunProgram();
        Console.WriteLine(arcade.screen.Values.Count(x => x == 2));
    }

    public override void Part2()
    {
        var arcade = new Arcade();
        var computer = new IntcodeComputer(lines[0])
        {
            output = arcade.Input,
            inputs = arcade.Output().GetEnumerator(),
            // debugger = true,
        };
        computer[0] = 2;
        computer.RunProgram();
        Console.WriteLine(arcade.score);
   }
}