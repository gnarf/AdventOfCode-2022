using AoC2019;

// See https://aka.ms/new-console-template for more information
if (args.Length < 1)
{
    Console.WriteLine("Please specify which day to run");
    return;
}

string day = "AoC2019.Day"+args[0];

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
    runner.Part1();
    Console.WriteLine("###### PART 2 ######");
    runner.Part2();
}
else
{
    Console.WriteLine(day + " not found");
}