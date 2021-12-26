using NUnit.Framework;
namespace AoC2019;

class Day4 : Puzzle
{

    public bool ValidPassword(int number)
    {
        string asString = number.ToString();
        bool twosame = false;
        for(int index=1; index<6; index++)
        {
            char prev = asString[index-1];
            if (asString[index]<asString[index-1]) return false;
            if (asString[index] == prev) twosame = true;
        }
        return twosame;
    }

    public bool MoreSecure(int number)
    {
        string asString = number.ToString();
        bool twosame = false;
        int same = 0;
        for(int index=1; index<6; index++)
        {
            char prev = asString[index-1];
            if (asString[index]<asString[index-1]) return false;
            if (asString[index] == prev) same++;
            else
            {
                if (same == 1) twosame = true;
                same = 0;
            } 
        }
        return twosame || same==1;
    }

    private int[]? _range = null;
    public int[] range => _range??=lines[0].Split('-').Select(s => Convert.ToInt32(s)).ToArray();

    public override void Part1()
    {
        int count = 0;
        for(int i=range[0];i<=range[1];i++)
        {
            if (ValidPassword(i))
            {
                count++;
            }
        }

        Console.WriteLine("Total " + count);

    }

    public override void Part2()
    {
        int count = 0;
        for(int i=range[0];i<=range[1];i++)
        {
            if (MoreSecure(i))
            {
                Console.WriteLine("Valid: "+i);
                count++;
            }
        }

        Console.WriteLine("Total " + count);

    }
}