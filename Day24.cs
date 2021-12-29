using NUnit.Framework;
namespace AoC2019;

class Day24 : Puzzle
{
    public bool[] gameState = new bool[25];

    public override void Parse(string filename)
    {
        base.Parse(filename);
        ParseLines();
    }

    public void ParseLines()
    {
        for (int y = 0; y<5; y++)
        {
            for (int x = 0; x<5; x++)
            {
                gameState[y*5 + x] = lines[y][x] == '#';
            }
        }
    }

    public bool newState(int index)
    {
        int neighbors = 0;
        if (index % 5 != 0 && gameState[index - 1]) neighbors++;
        if (index % 5 != 4 && gameState[index + 1]) neighbors++;
        if (index > 4  && gameState[index - 5]) neighbors++;
        if (index < 20 && gameState[index + 5]) neighbors++;
        if (gameState[index])
        {
            return neighbors == 1;
        }
        return neighbors == 1 || neighbors == 2;
    }

    public override void Part1()
    {
        HashSet<int> seen = new HashSet<int>{};
        while (true)
        {
            gameState = gameState.Select((s, i) => newState(i)).ToArray();
            var score = gameState.Reverse().Aggregate(0, (a,b) => a*2 + (b?1:0));
            if (seen.Contains(score))
            {
                Console.WriteLine("Duplicated state");
                Console.WriteLine(score);
                return;
            }
            seen.Add(score);
        }
    }

    public Dictionary<int, bool[]> pState = new Dictionary<int, bool[]>();
    public Dictionary<int, bool[]> sState = new Dictionary<int, bool[]>();

    public void SwapState()
    {
        pState = sState;
        sState = pState.Select(s => (s.Key, s.Value.ToArray())).ToDictionary(k => k.Key, e=>e.Item2);
    }

    public bool getstate(int index, int depth)
    {
        if (index == 12) throw new Exception("Illegal Index");
        bool[]? map;
        if (pState.TryGetValue(depth, out map))
        {
            return map[index];
        }
        return false;
    }
    public void setstate(int index, int depth, bool value)
    {
        if (index == 12) throw new Exception("Illegal Index");
        bool[]? map;
        if (sState.TryGetValue(depth, out map))
        {
            map[index] = value;
        }
        else if (value)
        {
            map = new bool[25];
            map[index] = value;
            sState[depth] = map;
        }
    }

    public int countNeighbors(int index, int depth)
    {
        if (index == 12) throw new Exception("Illegal Index");
        int neighbors = 0;
        if (index % 5 == 0)
        {
            // left up
            if (getstate(11,depth-1)) neighbors++;
        }
        else if (index == 13)
        {
            // left down
            if (getstate(4,depth+1)) neighbors++;
            if (getstate(9,depth+1)) neighbors++;
            if (getstate(14,depth+1)) neighbors++;
            if (getstate(19,depth+1)) neighbors++;
            if (getstate(24,depth+1)) neighbors++;
        }
        else
        {
            // left neighbor in grid
            if (getstate(index-1,depth)) neighbors++;
        }

        if (index % 5 == 4)
        {
            // right up
            if (getstate(13,depth-1)) neighbors++;
        }
        else if (index == 11)
        {
            // right down
            if (getstate(0,depth+1)) neighbors++;
            if (getstate(5,depth+1)) neighbors++;
            if (getstate(10,depth+1)) neighbors++;
            if (getstate(15,depth+1)) neighbors++;
            if (getstate(20,depth+1)) neighbors++;
        }
        else
        {
            // right neighbor in grid
            if (getstate(index+1,depth)) neighbors++;
        }

        if (index < 5)
        {
            // top up
            if (getstate(7,depth-1)) neighbors++;
        }
        else if (index == 17)
        {
            // top down
            if (getstate(20,depth+1)) neighbors++;
            if (getstate(21,depth+1)) neighbors++;
            if (getstate(22,depth+1)) neighbors++;
            if (getstate(23,depth+1)) neighbors++;
            if (getstate(24,depth+1)) neighbors++;
        }
        else
        {
            // top neighbor in grid
            if (getstate(index-5,depth)) neighbors++;
        }

        if (index >= 20)
        {
            // bottom up
            if (getstate(17,depth-1)) neighbors++;
        }
        else if (index == 7)
        {
            // bottom down
            if (getstate(0,depth+1)) neighbors++;
            if (getstate(1,depth+1)) neighbors++;
            if (getstate(2,depth+1)) neighbors++;
            if (getstate(3,depth+1)) neighbors++;
            if (getstate(4,depth+1)) neighbors++;
        }
        else
        {
            // bottom neighbor in grid
            if (getstate(index+5,depth)) neighbors++;
        }

        return neighbors;
    }

    public override void Part2()
    {
        // reset the gameState
        gameState = new bool[25];
        ParseLines();
        sState[0] = gameState;
        SwapState();

        for(int steps = 0; steps < 200; steps++)
        {
            for (int depth=pState.Keys.Min()-1; depth<=pState.Keys.Max()+1;depth++)
            {
                for (int index = 0; index < 25; index++)
                    if (index != 12)
                    {
                        bool setting = false;
                        var count = countNeighbors(index, depth);
                        if (getstate(index, depth))
                        {
                            setting = count == 1;
                        }
                        else
                        {
                            setting = count == 1 || count == 2;
                        }
                        // Console.WriteLine($"Count {count} in {index} @ {depth} -> {setting}");
                        setstate(index, depth, setting);
                    }
            }
            SwapState();
        }

        // 2152 too high
        Console.WriteLine(pState.Values.SelectMany(s=>s).Count(s=>s));
    }
}