using NUnit.Framework;
namespace AoC2019;

class Day16 : Puzzle
{

    public static int[] multiplier = new[] { 0, 1, 0, -1 };

    public byte[] FFT(byte[] input, int offset = 0)
    {
        byte[] output = new byte[input.Length];
        long spot = 0;
        for (int x = 0; x < input.Length; x++)
        {
            if (x == 0 || offset == 0)
            {
                spot = 0;
                for (int y = 0; y < input.Length; y++)
                {
                    if (offset > y)
                    {
                        spot += input[y];
                    }
                    else
                    {
                        spot += input[y] * multiplier[(y + offset + 1) / (x + offset + 1) % 4];
                    }
                }
            }
            else
            {
                spot = (spot + 10 - input[x-1])%10;
            }
            output[x] = (byte)(Math.Abs(spot) % 10);
        }
        return output;
    }

    int Chunk(byte[] input, int offset, int length)
    {
        var s = input.Skip(offset).Take(length).Select(c => c.ToString()).Aggregate("", (a, b) => a + b);
        return Convert.ToInt32(s);
    }

    public override void Part1()
    {
        byte[] initialCode = lines[0].ToCharArray().Select(c => (byte)(c - '0')).ToArray();
        byte[] code = initialCode;
        for (int phase = 0; phase < 100; phase++)
        {
            code = FFT(code);
        }
        Console.WriteLine(Chunk(code,0,8));
    }

    public override void Part2()
    {
        byte[] initialCode = Enumerable.Repeat(lines[0].ToCharArray().Select(c => (byte)(c - '0')), 10000).SelectMany(t=>t).ToArray();
        byte[] output = new byte[8];
        int offset = Chunk(initialCode, 0, 7);
        List<(int min, int max)> ranges = new List<(int, int)>();
        Console.WriteLine("Offset " + offset);
        byte[] code = initialCode.Skip(offset).ToArray();
        for (int phase = 0; phase < 100; phase++)
        {
            // Console.WriteLine("Phase " + phase);
            code = FFT(code, offset);
        }
        Console.WriteLine(Chunk(code,0,8));
    }
}