using NUnit.Framework;
namespace AoC2022;

class Day2 : Puzzle
{
    struct GameTurn
    {
        public Shape P1;
        public Shape P2;
    }

    enum Shape
    {
        Rock = 1,
        Paper = 2,
        Scissors = 3
    };


    List<GameTurn> turns = new List<GameTurn>();
    public override void Part1()
    {
        foreach (var line in lines)
        {
            var moves = line.Split(" ").Select(s => s switch { "A" => Shape.Rock, "B" => Shape.Paper, "C" => Shape.Scissors, "X" => Shape.Rock, "Y" => Shape.Paper, "Z" => Shape.Scissors, _ => throw new Exception("?") }).ToArray();
            turns.Add(new GameTurn{P1 = moves[0], P2 = moves[1] });
        }
        var score = 0;
        foreach (var turn in turns)
        {
            score += (int)turn.P2;
            if (turn.P1 == turn.P2)
            {
                score += 3;
            }
            else
            if (turn.P1 == Shape.Rock)
            {
                score += turn.P2 == Shape.Paper ? 6 : 0;
            }
            else
            if (turn.P1 == Shape.Paper)
            {
                score += turn.P2 == Shape.Scissors ? 6 : 0;
            }
            else
            if (turn.P1 == Shape.Scissors)
            {
                score += turn.P2 == Shape.Rock ? 6 : 0;
            }
        }
        Console.WriteLine(score);
    }

    public override void Part2()
    {
        var score = 0;
        foreach (var line in lines)
        {
            var moves = line.Split(" ");
            score += moves[1] switch
            {
                "X" => 0,
                "Y" => 3,
                "Z" => 6,
            };
            var shape = moves[0] switch { "A" => Shape.Rock, "B" => Shape.Paper, "C" => Shape.Scissors, _ => throw new Exception() };
            if (moves[1] == "Y")
            {
                score += (int)shape;
            }
            else
            if (shape == Shape.Rock)
            {
                score += moves[1] == "X" ? (int)Shape.Scissors : (int) Shape.Paper;
            }
            else
            if (shape == Shape.Paper)
            {
                score += moves[1] == "X" ? (int)Shape.Rock : (int) Shape.Scissors;
            }
            else
            if (shape == Shape.Scissors)
            {
                score += moves[1] == "X" ? (int)Shape.Paper : (int) Shape.Rock;
            }
        }
        Console.WriteLine(score);
    }
}