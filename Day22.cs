using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Drawing;

namespace AoC2022;

class Day22 : Puzzle
{
    public static Dictionary<Point2D, bool> board = new();
    public static List<int> steps = new();
    public static List<Turn2D> turns = new();
    public static int cubeSize;

    public override void Parse(string filename)
    {
        base.Parse(filename);

        cubeSize = (lines.Length - 2) / 4;
        for(int line=0; line<lines.Length - 2; line++)
        {
            for (int c = 0; c<lines[line].Length; c++)
            {
                if (lines[line][c] == '.')
                {
                    board.Add(new Point2D(c+1, line+1), false);
                }
                else if (lines[line][c] == '#')
                {
                    board.Add(new Point2D(c+1, line+1), true);
                }
            }
        }

        var directions = lines.Last();
        int lastTurn = -1;
        for (int c = 0; c<directions.Length; c++)
        {
            if (directions[c] >= '0' && directions[c] <= '9') continue;
            var y = directions.Substring(lastTurn+1, c - lastTurn - 1);
            steps.Add(int.Parse(y));
            lastTurn = c;
            if (directions[c] == 'R')
            {
                turns.Add(Turn2D.Right);
            }
            if (directions[c] == 'L')
            {
                turns.Add(Turn2D.Left);
            }
        }
        var x = directions.Substring(lastTurn+1);
        steps.Add(int.Parse(x));
    }

    public Point2D WrapPoint(Point2D p, Point2D direction)
    {
        var newPoint = p + direction;
        if (board.ContainsKey(newPoint)) return newPoint;
        IEnumerable<Point2D> sameDomain;
        if (direction.x == 0)
        {
            sameDomain = board.Keys.Where(k => k.x == p.x);
            // wrap around y
            if (direction.y < 0)
            {
                return sameDomain.Aggregate(Point2D.Max);
            }
            return sameDomain.Aggregate(Point2D.Min);
        }
        sameDomain = board.Keys.Where(k => k.y == p.y);
        // wrap around x
        if (direction.x < 0)
        {
            return sameDomain.Aggregate(Point2D.Max);
        }
        return sameDomain.Aggregate(Point2D.Min);
    }

    // public override void Part1()
    // {
    //     var spot = board.Keys.Where(k => k.y == 1).Aggregate(Point2D.Min);
    //     var facing = Point2D.Right;
    //     for (int x=0; x<steps.Count; x++)
    //     {
    //         for (int step = 0; step < steps[x]; step++)
    //         {
    //             var result = WrapPoint(spot, facing);
    //             if (board[result])
    //             {
    //                 Console.WriteLine($"Bonk");
    //                 break;
    //             }
    //             Console.WriteLine($"Stepping {spot}+{facing}={result}");
    //             spot = result;
    //         }
    //         if (x < turns.Count)
    //         {
    //             var result = facing.Turn(turns[x]);
    //             Console.WriteLine($"Turing {facing}+{turns[x]}={result}");
    //             facing = result;
    //         }
    //     }
    //     Console.WriteLine($"Ended maze at {spot} {facing}");
    //     var score = spot.y * 1000 + spot.x * 4;
    //     if (facing == Point2D.Up)
    //     {
    //         score += 3;
    //     }
    //     if (facing == Point2D.Left)
    //     {
    //         score += 2;
    //     }
    //     if (facing == Point2D.Down)
    //     {
    //         score += 1;
    //     }
    //     Console.WriteLine($"Final score {score}");
    // }


    // SERIOUSLY - Fuck off Eric Watsl for not using the same cube folding on the example as the real input
    //
    //   14
    //   2
    //  53
    //  6

    public int cubeFace(Point2D p)
    {
        if (p.y < 1) return -1;
        if (p.y <= cubeSize)
        {
            if (p.x <= cubeSize) return -1;
            if (p.x <= cubeSize * 2) return 1;
            if (p.x <= cubeSize * 3) return 4;
            if (p.x > cubeSize * 4) return -1;
        }
        if (p.y <= cubeSize * 2)
        {
            if (p.x <= cubeSize) return -1;
            if (p.x <= cubeSize * 2) return 2;
            return -1;
        }
        if (p.y <= cubeSize * 3)
        {
            if (p.x <= 0) return -1;
            if (p.x <= cubeSize) return 5;
            if (p.x <= cubeSize * 2) return 3;
            return -1;
        }
        if (p.y <= cubeSize * 4)
        {
            if (p.x <= 0) return -1;
            if (p.x <= cubeSize) return 6;
            return -1;
        }
        return -1;
    }

    public (Point2D spot, Point2D facing) WrapPointCube(Point2D spot, Point2D facing)
    {
        var startFace = cubeFace(spot);
        // Console.WriteLine($"CubeFace {startFace} {spot}");
        var testPoint = spot + facing;
        if (cubeFace(testPoint) == startFace)
        {
            return (testPoint, facing);
        }
        // we are leaving the cube face here, lets find out how.

        // normal walks with no cube crazy

        //    1144
        //    1144
        //    22
        //    22
        //  5533
        //  5533
        //  66
        //  66
        if (
            (cubeFace(testPoint) == 2 && startFace == 1) ||
            (cubeFace(testPoint) == 4 && startFace == 1) ||
            (cubeFace(testPoint) == 1 && startFace == 2) ||
            (cubeFace(testPoint) == 3 && startFace == 2) ||
            (cubeFace(testPoint) == 2 && startFace == 3) ||
            (cubeFace(testPoint) == 5 && startFace == 3) ||
            (cubeFace(testPoint) == 1 && startFace == 4) ||
            (cubeFace(testPoint) == 3 && startFace == 5) ||
            (cubeFace(testPoint) == 6 && startFace == 5) ||
            (cubeFace(testPoint) == 5 && startFace == 6)
        )
        {
            return (testPoint, facing);
        }

        //    1144
        //    1144
        //    22
        //    22
        //  5533
        //  5533
        //  66
        //  66

        if (startFace == 1)
        {
            if (facing == Point2D.Up)
            {
                // step to cube 6
                // top of 1 = left of 6
                var newFacing = facing.Turn(Turn2D.Right);
                var newPoint = new Point2D(1, cubeSize * 3 + (spot.x - cubeSize));
                Console.WriteLine($"Sanity {startFace} {spot} {facing} -> 6? {cubeFace(newPoint)} {newPoint} {newFacing}");
                return (newPoint, newFacing);
            }
            if (facing == Point2D.Left) //
            {
                // step to cube 5
                // left of 1 = left of 5
                var newFacing = facing.Turn(Turn2D.Around);
                var newPoint = new Point2D(1, cubeSize * 2 + 1 + (cubeSize - spot.y));
                Console.WriteLine($"Sanity {startFace} {spot} {facing} -> 5? {cubeFace(newPoint)} {newPoint} {newFacing}");
                return (newPoint, newFacing);
            }
        }
        if (startFace == 2)
        {
            if (facing == Point2D.Right)
            {
                // step to cube 4
                // right of 2 = bottom of 4
                var newFacing = facing.Turn(Turn2D.Left);
                var newPoint = new Point2D(spot.y + cubeSize, cubeSize);
                Console.WriteLine($"Sanity {startFace} {spot} {facing} -> 4? {cubeFace(newPoint)} {newPoint} {newFacing}");
                return (newPoint, newFacing);
            }
            if (facing == Point2D.Left)
            {
                // step to cube 5
                // left of 2 = top of 5
                var newFacing = facing.Turn(Turn2D.Left);
                var newPoint = new Point2D(spot.y - cubeSize, cubeSize * 2 + 1);
                Console.WriteLine($"Sanity {startFace} {spot} {facing} -> 5? {cubeFace(newPoint)} {newPoint} {newFacing}");
                return (newPoint, newFacing);
            }
        }
        if (startFace == 3)
        {
            if (facing == Point2D.Right) //
            {
                // step to cube 4
                // right of 2 = right of 4
                var newFacing = facing.Turn(Turn2D.Around);
                var newPoint = new Point2D(cubeSize * 3, (cubeSize * 3 - spot.y) + 1);
                Console.WriteLine($"Sanity {startFace} {spot} {facing} -> 4? {cubeFace(newPoint)} {newPoint} {newFacing}");
                return (newPoint, newFacing);
            }
            if (facing == Point2D.Down) //
            {
                // step to cube 6
                // bottom of 3 = right of 6
                var newFacing = facing.Turn(Turn2D.Right);
                var newPoint = new Point2D(cubeSize, cubeSize * 4 - (cubeSize * 2 - spot.x));
                Console.WriteLine($"Sanity {startFace} {spot} {facing} -> ?6 {cubeFace(newPoint)} {newPoint} {newFacing}");
                return (newPoint, newFacing);
            }
        }
        if (startFace == 4)
        {
            if (facing == Point2D.Right) //
            {
                // step to cube 3
                // right of 4 = right of 3
                var newFacing = facing.Turn(Turn2D.Around);
                var newPoint = new Point2D(cubeSize * 2, (cubeSize - spot.y) + cubeSize * 2 + 1);
                Console.WriteLine($"Sanity {startFace} {spot} {facing} -> 3? {cubeFace(newPoint)} {newPoint} {newFacing}");
                return (newPoint, newFacing);
            }
            if (facing == Point2D.Down)
            {
                // step to cube 2
                // bottom of 4 = right of 2
                var newFacing = facing.Turn(Turn2D.Right);
                var newPoint = new Point2D(cubeSize * 2, spot.x - cubeSize);
                Console.WriteLine($"Sanity {startFace} {spot} {facing} -> 2? {cubeFace(newPoint)} {newPoint} {newFacing}");
                return (newPoint, newFacing);
            }
            if (facing == Point2D.Up)
            {
                // step to cube 6
                // top of 4 = bottom of 6
                var newFacing = facing;
                var newPoint = new Point2D(spot.x - cubeSize * 2, cubeSize * 4);
                Console.WriteLine($"Sanity {startFace} {spot} {facing} -> 6? {cubeFace(newPoint)} {newPoint} {newFacing}");
                return (newPoint, newFacing);
            }
        }
        if (startFace == 5)
        {
            if (facing == Point2D.Left) //
            {
                // step to cube 1
                // left of 5 = left of 1
                var newFacing = facing.Turn(Turn2D.Around);
                var newPoint = new Point2D(cubeSize + 1, (cubeSize * 3 - spot.y) + 1);
                Console.WriteLine($"Sanity {startFace} {spot} {facing} -> 1? {cubeFace(newPoint)} {newPoint} {newFacing}");
                return (newPoint, newFacing);
            }
            if (facing == Point2D.Up)
            {
                // step to cube 2
                // top of 5 = left of 2
                var newFacing = facing.Turn(Turn2D.Right);
                var newPoint = new Point2D(cubeSize + 1 , spot.x + cubeSize);
                Console.WriteLine($"Sanity {startFace} {spot} {facing} -> 2? {cubeFace(newPoint)} {newPoint} {newFacing}");
                return (newPoint, newFacing);
            }
        }
        if (startFace == 6)
        {
            if (facing == Point2D.Right) //
            {
                // step to cube 3
                // right of 6 = bottom of 3
                var newFacing = facing.Turn(Turn2D.Left);
                var newPoint = new Point2D(spot.y - cubeSize*2, cubeSize * 3);
                Console.WriteLine($"Sanity {startFace} {spot} {facing} -> 3? {cubeFace(newPoint)} {newPoint} {newFacing}");
                return (newPoint, newFacing);
            }
            if (facing == Point2D.Down)
            {
                // step to cube 4
                // bottom of 6 = top of 4
                var newFacing = facing;
                var newPoint = new Point2D(spot.x + cubeSize * 2, 1);
                Console.WriteLine($"Sanity {startFace} {spot} {facing} -> 4? {cubeFace(newPoint)} {newPoint} {newFacing}");
                return (newPoint, newFacing);
            }
            if (facing == Point2D.Left) //
            {
                // step to cube 1
                // left of 6 = top of 6
                var newFacing = facing.Turn(Turn2D.Left);
                var newPoint = new Point2D(spot.y - cubeSize *2, 1);
                Console.WriteLine($"Sanity {startFace} {spot} {facing} -> 1? {cubeFace(newPoint)} {newPoint} {newFacing}");
                return (newPoint, newFacing);
            }
        }
        throw new Exception("Unknown cube face transition");
    }


    public override void Part2()
    {
         var spot = board.Keys.Where(k => k.y == 1).Aggregate(Point2D.Min);
        var facing = Point2D.Right;
        var lastVisit = new Dictionary<Point2D, Point2D>();
        for (int x=0; x<steps.Count; x++)
        {
            for (int step = 0; step < steps[x]; step++)
            {
                lastVisit[spot] = facing;
                var (result, newFacing) = WrapPointCube(spot, facing);
                if (board[result])
                {
                    Console.WriteLine($"Bonk");
                    break;
                }
                Console.WriteLine($"{cubeFace(spot)} {spot} {facing} -> {cubeFace(result)} {result} {newFacing}");
                spot = result;
                facing = newFacing;
                lastVisit[spot] = facing;
            }
            if (x < turns.Count)
            {
                var result = facing.Turn(turns[x]);
                Console.WriteLine($"Turing {facing}+{turns[x]}={result}");
                facing = result;
            }
        }

        var max = board.Keys.Aggregate(Point2D.Max);
        for (int y=1; y<=max.y; y++)
        {
            var s = "";
            for (int x=1; x<=max.x; x++)
            {
                if (board.TryGetValue(new Point2D(x, y), out var wall))
                {
                    if (wall) s += '#';
                    else
                    if (lastVisit.TryGetValue(new Point2D(x, y), out var face))
                    {
                        if (face == Point2D.Down) s+= 'v';
                        if (face == Point2D.Up) s+= '^';
                        if (face == Point2D.Right) s+='>';
                        if (face == Point2D.Left) s+='<';
                    }
                    else s += '.';
                }
                else
                {
                    s += ' ';
                }
            }
            Console.WriteLine(s);
        }

        Console.WriteLine($"Ended maze at {spot} {facing}");
        var score = spot.y * 1000 + spot.x * 4;
        if (facing == Point2D.Up)
        {
            score += 3;
        }
        if (facing == Point2D.Left)
        {
            score += 2;
        }
        if (facing == Point2D.Down)
        {
            score += 1;
        }
        // 42239 too low
        // 83215 too high
        Console.WriteLine($"Final score {score}");
   }
}