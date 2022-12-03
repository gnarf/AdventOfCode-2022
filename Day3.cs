using NUnit.Framework;
namespace AoC2022;

class Day3 : Puzzle
{
    public override void Part1()
    {
        Console.WriteLine(lines.Select(s =>
        {
            var half = s.Length / 2;
            var first = s.Substring(0, half);
            var second = s.Substring(half);
            var used = new HashSet<char>();
            for(int x=0;x<half; x++)
            {
                used.Add(first[x]);
            }
            for(int x=0;x<half; x++)
            {
                if (used.Contains(second[x]))
                {
                    if (second[x] >= 'a' && second[x] <='z') return (int) (second[x]-'a') + 1;
                    return (int) (second[x] - 'A') + 27;
                }
            }
            return 0;
            
        }).Sum());
    }

    public override void Part2()
    {  
        HashSet<char>[] used = new HashSet<char>[3];
        for (int x=0; x<3; x++) used[x] = new HashSet<char>();
        
        Console.WriteLine(lines.Select((s,i) =>
        {
            foreach (var c in s) { used[i%3].Add(c); }
            if (i%3 == 2)
            {
                var result = used[0].Intersect(used[1]).Intersect(used[2]).FirstOrDefault();
                used[0].Clear();
                used[1].Clear();
                used[2].Clear();
                Console.WriteLine(result);
                if (result >= 'a' && result <= 'z') return (int)(result - 'a')+1;
                return (int)(result - 'A')+27;
            }
            return 0;
        }).Sum());
    }
}