using NUnit.Framework;
namespace AoC2019;

class Day8 : Puzzle
{
    int[][][] ParseSIF(string image, int width, int height)
    {
        int layers = image.Length / (width * height);

        int[][][] output = new int[layers][][];
        for(int l = 0; l<layers; l++)
        {
            output[l] = new int[width][];
            for(int x=0; x<width; x++)
            {
                output[l][x] = new int[height];
                for (int y=0;y<height;y++)
                {
                    output[l][x][y]=image[x + (y*width) + (l*width*height)]-'0';
                }
            }
        }


        return output;
    }
    public override void Part1()
    {
        var image = ParseSIF(lines[0],25,6);
        int fewestZero = int.MaxValue;
        foreach (var layer in image)
        {
            var counts = layer.SelectMany(c=>c).GroupBy(c=>c).ToDictionary(g=>g.Key, elementSelector: g=>g.Count());

            int countZero;
            counts.TryGetValue(0, out countZero);
            if (countZero>0 && countZero<fewestZero)
            {
                Console.WriteLine("Layer count zero " + countZero);
                fewestZero = countZero;
                Console.WriteLine(counts[1]*counts[2]);
            }
        }
    }

    public override void Part2()
    {
        var image = ParseSIF(lines[0],25,6);
        for(int y=0; y<6; y++)
        {
            for(int x=0; x<25; x++)
            {
                for(int l=0; l<image.Length; l++)
                {
                    if (image[l][x][y]==1) { Console.Write('#'); break; }
                    if (image[l][x][y]==0) { Console.Write(' '); break; }
                }
            }
            Console.WriteLine();
        }
    }
}