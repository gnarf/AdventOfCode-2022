using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Numerics;
namespace AoC2019;


class Day22 : Puzzle
{

    public int[] SpaceCards = new int[0];

    public override void Part1()
    {
        SpaceCards = Enumerable.Range(0, 10007).ToArray();
        int[] extraStack = new int[SpaceCards.Length];
        foreach (var line in lines)
        {
            var match = Regex.Match(line, @"deal with increment (\d+)");
            if (match.Success)
            {
                var number = Convert.ToInt32(match.Groups[1].Value);
                int place = 0;
                for (int x=0; x<SpaceCards.Length; x++)
                {
                    extraStack[place] = SpaceCards[x];
                    place = (place + number) % SpaceCards.Length;
                }
                Array.Copy(extraStack, SpaceCards, SpaceCards.Length);
                continue;
            }
            match = Regex.Match(line, @"cut (-?\d+)");
            if (match.Success)
            {
                var number = Convert.ToInt32(match.Groups[1].Value);

                int place = number < 0 ? SpaceCards.Length + number : number;
                SpaceCards = SpaceCards.Skip(place).Concat(SpaceCards.Take(place)).ToArray();
                continue;
            }
            match = Regex.Match(line, @"deal into new stack");
            if (match.Success)
            {
                SpaceCards = SpaceCards.Reverse().ToArray();
                continue;
            }
            throw new Exception("Unknown line: " + line);
        }
        Console.WriteLine("Find card 2019: " + SpaceCards.ToList().IndexOf(2019));
    }

    public override void Part2()
    {
        long deckSize = 119315717514047;
        long shuffleTimes = 101741582076661;
        long position = 2020;

        // long deckSize = 10007;
        // long position = 6061;
        // long shuffleTimes = 1;

        // build up the formula:
        // y = ( ax + b ) mod Z
        BigInteger a = 1, b = 0;

        // We feed each formula into it's previous row.
        // We are inverting the order because our initial "x" is the "final position"
        //
        // y = (ax + b) mod deckSize
        foreach (var line in lines.Reverse())
        {
            var match = Regex.Match(line, @"^deal with increment (\d+)");
            if (match.Success)
            {
                var number = Convert.ToInt32(match.Groups[1].Value);
                // going forward this is just a*=n;b*=n; but now we need the "inverse" of this operation
                // WIKI: A simple consequence of Fermat's little theorem is that if p is prime, then a^−1 ≡ a^(p−2) (mod p)
                // so, instead of a / n (which wouldn't be an integer) we can a * (n ^ (deckSize - 2))
                var p = BigInteger.ModPow(number, deckSize - 2, deckSize);
                a = a * p;
                b = b * p;
            }
            match = Regex.Match(line, @"^cut (-?\d+)");
            if (match.Success)
            {
                var number = Convert.ToInt64(match.Groups[1].Value);
                // inverse cut
                b = b + number;
            }
            match = Regex.Match(line, @"^deal into new stack");
            if (match.Success)
            {
                // inverse!
                a = a * -1;
                // invert, and shift (0 goes to decksize - 1)
                b = ( b + 1 ) * -1;
            }
            // force everything modulo positive just for sanity
            a = ((a %deckSize) + deckSize)%deckSize;
            b = ((b %deckSize) + deckSize)%deckSize;
            // Console.WriteLine($"{line}: y = {a}x + {b} mod {deckSize}");
        }

        // reducing y = ax + b mod decksize over shuffleTimes
        // the "a" part is easy - its just a^shuffletimes
        // the "b" part becomes the sum of all b + b*a + b*a*a, etc -> b * a geometric progression of a over suffleTimes
        // y = (a^shuffleTimes)*x + (b * (1 - a^shuffleTimes) / (1 - a))
        // we can't use division here so modulo multiplicitive inverse again
        var pow = BigInteger.ModPow(a, shuffleTimes, deckSize);
        var r = (
            (pow * position) + 
            (
                (b * (1 - pow)) *
                (BigInteger.ModPow(1 - a, deckSize - 2, deckSize))
            )
        ) % deckSize;
        Console.WriteLine(r);
    }
}