using NUnit.Framework;
namespace AoC2019;

class Day17 : Puzzle
{
    public class CameraReader
    {
        public Dictionary<Point2D, char> screen = new Dictionary<Point2D, char>();
        public List<Point2D> scaffolds = new List<Point2D>();

        public int x;
        public int y;
        public Point2D robot = Point2D.zero;
        public int width;
        public int height;

        public void Input(long data)
        {
            char c = (char)data;
            if (c=='\n')
            {
                y++;
                x=0;
            }
            else
            {
                if (c!='.')
                {
                    scaffolds.Add(new Point2D(x, y));
                    if (c!='#') robot = new Point2D(x, y);
                }                
                screen[new Point2D(x, y)] = c;
                width = Math.Max(x, width);
                height = Math.Max(y, height);
                x++;                
            }
        }
        public void PrintGrid()
        {
            var min = screen.Keys.Aggregate(Point2D.Min);
            var max = screen.Keys.Aggregate(Point2D.Max);

            for (int y = min.y; y <= max.y; y++)
            {
                Console.Write($"{y:+00;-00}:");
                for (int x = min.x; x <= max.x; x++)
                {
                    Console.Write(screen[new Point2D(x,y)]);
                }
                Console.WriteLine();
            }
        }

    }
    public override void Part1()
    {
        var cam = new CameraReader();
        var computer = new IntcodeComputer(lines[0])
        {
            output = cam.Input
        };
        computer.RunProgram();
        // cam.PrintGrid();
        var intersetcs = cam.scaffolds.Where(s => Day15.FacingVector.Values.Count(v => cam.scaffolds.Contains(v + s)) >= 3).ToList();
        Console.WriteLine(intersetcs.Sum(a => a.x * a.y));
    }

    public enum Turn { Left, Right }

    public static Dictionary<char, Facing2D> charFacing = new Dictionary<char, Facing2D>
    {
        {'^', Facing2D.Up},
        {'>', Facing2D.Right},
        {'v', Facing2D.Down},
        {'<', Facing2D.Left}
    };

    public static Dictionary<Facing2D, Point2D> facingVector = new Dictionary<Facing2D, Point2D>
    {
        {Facing2D.Up, new Point2D(0, -1)},
        {Facing2D.Right, new Point2D(1, 0)},
        {Facing2D.Down, new Point2D(0, 1)},
        {Facing2D.Left, new Point2D(-1, 0)}
    };

    public static Dictionary<Facing2D, Facing2D> LeftTurn => Day11.LeftTurn;
    public static Dictionary<Facing2D, Facing2D> RightTurn => Day11.RightTurn;

    public class RobotController
    {
        public Point2D position = Point2D.zero;
        public Facing2D facing;
        public List<(Turn turn, int dist)> instructions = new List<(Turn turn, int dist)>();
        public CameraReader camera;
        public List<Point2D> scaffolds => camera.scaffolds;
        public string commands = "";

        public RobotController(CameraReader cam)
        {
            position = cam.robot;
            facing = charFacing[cam.screen[position]];
            camera = cam;
        }

        public void WalkPath()
        {
            Turn turn = Turn.Left;
            if (scaffolds.Contains(position+facingVector[LeftTurn[facing]]))
            {
                facing = LeftTurn[facing];
            }
            else if (scaffolds.Contains(position+facingVector[RightTurn[facing]]))
            {
                facing = RightTurn[facing];                
                turn = Turn.Right;
            }
            else
            {
                return;
            }
            var v = facingVector[facing];
            int depth = 2;
            while(scaffolds.Contains(position + depth * v)) depth++;
            instructions.Add((turn, depth - 1));
            position = position + v * (depth-1);
            WalkPath();
        }

        public string StepToString((Turn turn, int dist) move)
        {
            if (move.turn == Turn.Left) return $"L,{move.dist}";
            return $"R,{move.dist}";
        }


        public void ReducePath()
        {
            List<string> paths = instructions.Select(StepToString).ToList();
            List<List<string>> compressed = new List<List<string>>();
            for(int left = 0; left < paths.Count; left++)
            {
                int remainingSteps = paths.Count - left;
                if (paths[left].Length==1) continue;

                for (int right = left + (remainingSteps / 2); right > left; right--)
                {
                    int size = right - left + 1;
                    if (paths.Skip(left).Take(size).Any(s=>s.Length==1)) continue;
                    bool found = false;
                    for (int test = right; test + size - 1 < paths.Count; test++ )
                    {
                        if (paths.Skip(test).Take(size).SequenceEqual(paths.Skip(left).Take(size)))
                        {
                            if (!found)
                            {
                                compressed.Add(paths.Skip(left).Take(size).ToList());
                                Console.WriteLine("Found " + compressed[^1].Aggregate("",(a,b)=>a+b+','));
                            }
                            paths.RemoveRange(test, size);
                            paths.Insert(test, "" + (char)((int)(compressed.Count-1)+'A'));
                            found = true; 
                        }
                    }
                    if (found)
                    {
                        paths.RemoveRange(left, size);
                        paths.Insert(left, "" + (char)((int)(compressed.Count-1)+'A'));
                        goto nextLeft;
                    }
                }
                nextLeft:;
            }

            commands = "" + paths.Aggregate((a,b) => a+','+b) + "\n";
            foreach(var path in compressed)
            {
                commands += path.Aggregate((a,b) => a+','+b) + "\n";
            }
            commands += "n\n";
            return;
        }

        public IEnumerable<long> SendASCII()
        {
            for(int index = 0;index<commands.Length;index++)
            {
                yield return (long)commands[index];
            }            
        }

        public long lastInput = 0;
        public void Input(long v) => lastInput = v;
    }

    public override void Part2()
    {
        var cam = new CameraReader();
        var computer = new IntcodeComputer(lines[0])
        {
            output = cam.Input
        };
        computer.RunProgram();
        // cam.PrintGrid();
        var robot = new RobotController(cam);
        robot.WalkPath();
        robot.ReducePath();

        computer = new IntcodeComputer(lines[0])
        {
            output = robot.Input,
            inputs = robot.SendASCII().GetEnumerator()
        };
        computer[0]=2;
        computer.RunProgram();
        Console.WriteLine(robot.lastInput);
    }
}