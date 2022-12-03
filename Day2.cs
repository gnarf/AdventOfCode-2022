using NUnit.Framework;
namespace AoC2022;

class Day2 : Puzzle
{
    struct GameTurn
    {
        public Shape P1;
        public Shape P2;

        public GameResult result
        {
            get
            {
                if (P1 == P2) return GameResult.Draw;
                return P2 == Beats(P1) ? GameResult.Win : GameResult.Lose;
            }
        }

        public int turnTotalScore => (int) P2 + (int) result;
    }

    enum GameResult
    {
        Win = 6,
        Draw = 3,
        Lose = 0,
    };

    public enum Shape
    {
        Rock = 1,
        Paper = 2,
        Scissors = 3
    };

    public static Shape Beats(Shape X) => X switch
    {
        Shape.Rock => Shape.Paper,
        Shape.Paper => Shape.Scissors,
        Shape.Scissors => Shape.Rock,
        _ => throw new Exception(),
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
            score += turn.turnTotalScore;
        }
        Console.WriteLine(score);
    }

    public override void Part2()
    {
        var score = 0;
        foreach (var line in lines)
        {
            var moves = line.Split(" ");
            var strat = moves[1] switch
            {
                "X" => GameResult.Lose,
                "Y" => GameResult.Draw,
                "Z" => GameResult.Win,
                _ => throw new Exception(),
            };
            var P1 = moves[0] switch {
                "A" => Shape.Rock,
                "B" => Shape.Paper,
                "C" => Shape.Scissors,
                _ => throw new Exception()
            };
            var P2 = strat switch
            {
                GameResult.Win => Beats(P1),
                GameResult.Draw => P1,
                GameResult.Lose => Beats(Beats(P1)),
                _ => throw new Exception(),
            };
            var turn = new GameTurn{P1 = P1, P2 = P2};
            score += turn.turnTotalScore;
       }
        Console.WriteLine(score);
    }
}