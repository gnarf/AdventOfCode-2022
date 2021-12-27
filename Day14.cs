using NUnit.Framework;
namespace AoC2019;

class Day14 : Puzzle
{

    struct Component
    {
        public int quantity;
        public string resource;

        public override string ToString()
        {
            return $"{resource} {quantity}";
        }

        public static Component Parse(string p)
        {
            var parts = p.Split(" ");
            return new Component { quantity = Convert.ToInt32(parts[0]), resource = parts[1] };
        }
    }

    public override void Parse(string filename)
    {
        base.Parse(filename);
        ParseLines();
    }

    Dictionary<string, (Component optput, Component[] input)> map = new Dictionary<string, (Component, Component[])>();
    public void ParseLines()
    {
        map.Clear();
        foreach (var line in lines)
        {
            var sp = line.Split(" => ");
            var output = Component.Parse(sp[1]);
            var input = sp[0].Split(", ").Select(Component.Parse).ToArray();
            map[output.resource] = (output, input);
        }
    }



    public override void Part1()
    {
        Dictionary<string, int> resourceNeeded = new Dictionary<string, int>(map.Count)
        {
            {"FUEL", 1}
        };
        Dictionary<string, int> resourceCreated = new Dictionary<string, int>(map.Count);
        bool loop = true;
        while (loop)
        {
            loop = false;
            foreach (var (resource, needed) in resourceNeeded.ToArray())
            {
                if (resource == "ORE") continue;
                var (output, input) = map[resource];
                if (!resourceCreated.ContainsKey(resource)) resourceCreated[resource] = 0;
                // how much do we STILL need?
                int reallyNeeded = needed - resourceCreated[resource];
                if (reallyNeeded <= 0) continue;

                int outputsNeeded = (int)Math.Ceiling(reallyNeeded / (double)output.quantity);

                // create the resource
                foreach (var component in input)
                {
                    if (resourceNeeded.ContainsKey(component.resource))
                    {
                        resourceNeeded[component.resource] = resourceNeeded[component.resource] + component.quantity * outputsNeeded;
                    }
                    else
                    {
                        resourceNeeded[component.resource] = component.quantity * outputsNeeded;
                    }
                }
                resourceCreated[resource] = resourceCreated[resource] + outputsNeeded * output.quantity;
                loop = true;
            }
        }
        Console.WriteLine(resourceNeeded["ORE"]);
    }

    public long OreNeeded(long fuel)
    {
        Dictionary<string, long> resourceNeeded = new Dictionary<string, long>(map.Count + 1)
        {
            {"FUEL", fuel},
            {"ORE", 0}
        };
        Dictionary<string, long> resourceCreated = new Dictionary<string, long>(map.Count);
        bool loop = true;
        while (loop)
        {
            loop = false;
            foreach (var (resource, needed) in resourceNeeded.ToArray())
            {
                if (resource == "ORE") continue;
                var (output, input) = map[resource];
                if (!resourceCreated.ContainsKey(resource)) resourceCreated[resource] = 0;
                // how much do we STILL need?
                long reallyNeeded = needed - resourceCreated[resource];
                if (reallyNeeded <= 0) continue;

                long outputsNeeded = (long)Math.Ceiling(reallyNeeded / (double)output.quantity);

                // create the resource
                foreach (var component in input)
                {
                    if (resourceNeeded.ContainsKey(component.resource))
                    {
                        resourceNeeded[component.resource] = resourceNeeded[component.resource] + component.quantity * outputsNeeded;
                    }
                    else
                    {
                        resourceNeeded[component.resource] = component.quantity * outputsNeeded;
                    }
                }
                resourceCreated[resource] = resourceCreated[resource] + outputsNeeded * output.quantity;
                loop = true;
            }
        }
        return resourceNeeded["ORE"];
    }

    public override void Part2()
    {
        // rather than be extra clever here, maybe just binary searching all fuel requirements will be fast enough
        long top = 1000000000000;
        long bottom = 1;

        while (Math.Abs(top - bottom) > 1)
        {
            long test = (top + bottom) / 2;
            long ore = OreNeeded(test);
            if (ore > 1000000000000)
            {
                top = test;
            }
            else
            {
                bottom = test;
            }
        }
        Console.WriteLine(bottom);
    }
}