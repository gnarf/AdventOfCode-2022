using NUnit.Framework;
namespace AoC2019;

class Day6 : Puzzle
{

    struct Orbit
    {
        public string parent;
    }

    Dictionary<string, Orbit> orbits = new Dictionary<string, Orbit>();

    public void ParseLines()
    {
        orbits = new Dictionary<string, Orbit>();
        foreach (var line in lines)
        {
            var parts = line.Split(')');
            orbits[parts[1]] = new Orbit { parent = parts[0] };
        }
    }

    public override void Part1()
    {
        ParseLines();
        int direct = lines.Length;
        int indirect = 0;
        foreach (var orbit in orbits.Values)
        {
            var test = orbit;
            while (orbits.ContainsKey(test.parent))
            {
                indirect++;
                test = orbits[test.parent];
            }
        }
        Console.WriteLine(direct + indirect);
    }

    public IEnumerable<string> Parents(string start)
    {
        Orbit test = orbits[start];
        while (orbits.ContainsKey(test.parent)) {
            yield return test.parent;
            test = orbits[test.parent];
        }
    }

    public override void Part2()
    {
        ParseLines();
        var start = orbits["YOU"].parent;
        var dest = orbits["SAN"].parent;

        var sharedParent = Parents(start).Intersect(Parents(dest)).FirstOrDefault();
        Console.WriteLine("Shared Parent " + sharedParent);
        Console.WriteLine( 2 + Parents(start).TakeWhile(c=>c!=sharedParent).Count() + Parents(dest).TakeWhile(c=>c!=sharedParent).Count() );

    }
}