using System.Runtime.CompilerServices;
using System.Threading.Channels;
using NUnit.Framework;
namespace AoC2019;

class Day7 : Puzzle
{


    [TestCase("3,15,3,16,1002,16,10,16,1,16,15,15,4,15,99,0,0", new long[] { 4, 3, 2, 1, 0 }, ExpectedResult = 43210)]
    public static long Reduce(string program, long[] eachInput)
    {
        long memo = 0;

        foreach (var input in eachInput)
        {
            bool alreadySet = false;
            void SetMemo(long value)
            {
                if (alreadySet) throw new Exception("attempt to set output twice");
                alreadySet = true;
                memo = value;
            }
            var computer = new IntcodeComputer(program)
            {
                inputs = (new List<long> { input, memo }).GetEnumerator(),
                output = SetMemo
            };
            computer.RunProgram();
        }
        return memo;
    }

    [TestCase("3,26,1001,26,-4,26,3,27,1002,27,2,27,1,27,26,27,4,27,1001,28,-1,28,1005,28,6,99,0,0,5", new long[] { 9,8,7,6,5 }, ExpectedResult = 139629729)]
    public static long ReduceRecurse(string program, long[] eachInput)
    {
        // Console.WriteLine("Running");
        List<System.Threading.Channels.Channel<long>> storage = eachInput.Select(input=>{
            Channel<long> result = Channel.CreateBounded<long>(2);
            result.Writer.TryWrite(input);
            return result;
        }).ToList();

        CancellationTokenSource cancelled = new CancellationTokenSource();

        List<long> LastOutputs = new List<long>(eachInput);

        var computers = eachInput.Select((value, index) => {
            void Output(long value)
            {
                // Console.WriteLine(String.Format("Output from {0}: {1}", index, value));
                LastOutputs[index] = value;
                storage[(index+1)%eachInput.Length].Writer.TryWrite(value);
                // Send a value through the IO pipe??
            }

            async IAsyncEnumerable<long> Input([EnumeratorCancellation] CancellationToken cancellationToken = default)
            {
                while (true)
                {
                    long value = await storage[index].Reader.ReadAsync();
                    // Console.WriteLine("Input to {0}: {1}", index, value);
                    yield return value;
                }
            }

            return new IntcodeComputer(program)
            {
                inputsAsync = Input().GetAsyncEnumerator(cancelled.Token),
                output = Output
            };
        }).ToList();

        var running = computers.Select(computer => computer.RunProgramAsync());
        // pump the 0 into 0
        storage[0].Writer.TryWrite(0);
        
        Task.WaitAll(running.ToArray());
        // computers[eachInput.Length - 1].output(0);
        storage.ForEach(s=>s.Writer.Complete());
        // Console.WriteLine("Result: " + running.Last().Result);
        cancelled.Cancel();

        return LastOutputs[4];
    }

    public IEnumerable<long[]> AllInputs()
    {
        List<int> values = new List<int> { 0, 1, 2, 3, 4 };
        foreach (var v1 in values)
        {
            foreach (var v2 in values.Where(v => v != v1))
            {
                foreach (var v3 in values.Where(v => v != v2 && v != v1))
                {
                    foreach (var v4 in values.Where(v => v != v2 && v != v1 && v != v3))
                    {
                        yield return new long[]{values.Find(v => v != v2 && v != v1 && v != v3 && v != v4), v4, v3, v2, v1};
                    }
                }
            }
        }
    }
    public IEnumerable<long[]> AllInputs2()
    {
        List<int> values = new List<int> { 5, 6, 7, 8, 9 };
        foreach (var v1 in values)
        {
            foreach (var v2 in values.Where(v => v != v1))
            {
                foreach (var v3 in values.Where(v => v != v2 && v != v1))
                {
                    foreach (var v4 in values.Where(v => v != v2 && v != v1 && v != v3))
                    {
                        yield return new long[]{values.Find(v => v != v2 && v != v1 && v != v3 && v != v4), v4, v3, v2, v1};
                    }
                }
            }
        }
    }

    public override void Part1()
    {
        long[] bestInputs = new long[0];
        long bestResult = 0;
        foreach(var inputs in AllInputs())
        {
            long result = Reduce(lines[0], inputs);
            if (result > bestResult)
            {
                bestResult = result;
                bestInputs = inputs;
                Console.WriteLine("Found a better result: " + result + " code " + bestInputs.Aggregate( "", (memo, value) => memo + value.ToString() ));
            }
        }
    }

    public override void Part2()
    {
        // 272368 -- too low
        long[] bestInputs = new long[0];
        long bestResult = 0;
        foreach(var inputs in AllInputs2())
        {
            long result = ReduceRecurse(lines[0], inputs);
            if (result > bestResult)
            {
                bestResult = result;
                bestInputs = inputs;
                Console.WriteLine("Found a better result: " + result + " code " + bestInputs.Aggregate( "", (memo, value) => memo + value.ToString() ));
            }
        }
    }
}