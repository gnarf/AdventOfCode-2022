using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
namespace AoC2022;

class Day7 : Puzzle
{
    public List<string> Directories = new();
    public Dictionary<string, long> FileSizes = new();

    public void ParseFilesizes()
    {
        if (FileSizes.Count() == 0)
        {
            List<string> DirectoryStack = new List<string>();
            string FullPath(string last) => DirectoryStack.Append(last).Aggregate("/", (a, b) => a + b + "/");
            foreach (var line in lines)
            {
                if (line[0] == '$')
                {
                    var command = line.Substring(2).Split(' ');
                    if (command[0] == "cd")
                    {
                        if (command[1] == "..") DirectoryStack.RemoveAt(DirectoryStack.Count - 1);
                        else if (command[1] == "/") DirectoryStack.Clear();
                        else DirectoryStack.Add(command[1]);
                    }
                }
                else
                {
                    var parts = line.Split(' ');
                    if (parts[0] == "dir")
                    {
                        Directories.Add(FullPath(parts[1]));
                    }
                    else
                    {
                        FileSizes.Add(FullPath(parts[1]).TrimEnd('/'), Convert.ToInt64(parts[0]));
                    }
                }
            }
        }

    }

    public override void Part1()
    {
        ParseFilesizes();
        var result = Directories.Select( dir => FileSizes.Where(kv => kv.Key.StartsWith(dir)).Select(kv => kv.Value).Sum() )
            .Where(s => s < 100000)
            .Sum();
        Console.WriteLine(result);
    }

    public override void Part2()
    {
        ParseFilesizes();
        long diskSize = 70000000;
        var total = FileSizes.Values.Sum();
        var free = diskSize - total;
        var needed = 30000000 - free;
        var dirs = Directories.Select( dir => FileSizes.Where(kv => kv.Key.StartsWith(dir)).Select(kv => kv.Value).Sum() );
        var smallest = dirs.Where(s => s > needed).Min();
        Console.WriteLine(smallest);

    }
}