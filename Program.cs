﻿using AoC2022;

// See https://aka.ms/new-console-template for more information
if (args.Length < 1)
{
    Console.WriteLine("Please specify which day to run");
    return;
}

string day = "AoC2022.Day"+args[0];

Console.WriteLine("looking for " + day);
Type? RunnerType = System.Reflection.Assembly.GetExecutingAssembly().GetType(day);
if (RunnerType!=null && Activator.CreateInstance(RunnerType) is Puzzle runner)
{
    string inputFile = "input/"+args[0];
    if (args.Length > 1)
    {
        inputFile += "."+args[1];
    }
    inputFile += ".txt";
    runner.Parse(inputFile);
    Console.WriteLine("###### PART 1 ######");
    var clock = System.Diagnostics.Stopwatch.StartNew();
    runner.Part1();
    clock.Stop();
    Console.WriteLine(clock.Elapsed);
    Console.WriteLine("###### PART 2 ######");
    clock = System.Diagnostics.Stopwatch.StartNew();
    runner.Part2();
    clock.Stop();
    Console.WriteLine(clock.Elapsed);
}
else
{
    Console.WriteLine(day + " not found");
}