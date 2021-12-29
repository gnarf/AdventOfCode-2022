using NUnit.Framework;
using System.Threading.Channels;
namespace AoC2019;

class Day23 : Puzzle
{
    public static List<Channel<(long,long)>> channels = new List<Channel<(long,long)>>();
    public static List<NIC> nics = new List<NIC>();

    public static HashSet<long> SeenYs = new HashSet<long>();

    public class NAT
    {
        public static long X;
        public static long Y;
    }

    public class NIC
    {
        public IntcodeComputer computer;
        public long address;

        public NIC(long address)
        {
            computer = new IntcodeComputer(lines[0])
            {
                inputsAsync = Input().GetAsyncEnumerator(),
                output = Output
            };
            this.address = address;
        }

        public bool reading = true;

        public async IAsyncEnumerable<long> Input()
        {
            yield return address;
            var reader = channels[(int)address].Reader;
            while (true)
            {
                (long, long) value = (0,0);
                if (reader.TryRead(out value))
                {
                    reading = true;
                    yield return value.Item1;
                    yield return value.Item2;
                }
                else
                {
                    reading = false;
                    yield return -1;
                }
            }
        }

        public int nextState = 0;
        public long messageDest = 0;
        public long messageX = 0;

        public void Output(long value)
        {
            if (nextState == 0)
            {
                messageDest = value;
            }
            else if (nextState == 1)
            {
                messageX = value;
            }
            else
            {
                if (messageDest == 255)
                {
                    // Console.WriteLine($"{messageDest}: {messageX} {value}");
                    NAT.X = messageX;
                    NAT.Y = value;
                }
                else
                {
                    channels[(int)messageDest].Writer.TryWrite((messageX, value));
                }
            }
            nextState = (nextState + 1 ) % 3;
        }
    }

    public override void Part1()
    {
        foreach(var i in Enumerable.Range(0, 50))
        {
            channels.Add(Channel.CreateUnbounded<(long,long)>());
            nics.Add(new NIC(i));
        }
        NAT.Y = long.MinValue;
        var tokenSource2 = new CancellationTokenSource();
        CancellationToken ct = tokenSource2.Token;

        var threads = nics.Select(n => Task.Run(() => n.computer.RunProgramAsync(ct))).ToArray();

        while(NAT.Y == long.MinValue){
            Console.WriteLine(nics.Where(n => n.reading).Select(n=>n.address).Aggregate("", (a,b) => a+b+","));
            Thread.Sleep(100);
        } 
        tokenSource2.Cancel();
        try {
            Task.WaitAll(threads);
        }
        catch {}
        Console.WriteLine("Done with part 1: " + NAT.Y);
    }

    public override void Part2()
    {
        channels.Clear();
        nics.Clear();
        foreach(var i in Enumerable.Range(0, 50))
        {
            channels.Add(Channel.CreateUnbounded<(long,long)>());
            nics.Add(new NIC(i));
        }
        NAT.Y = long.MinValue;
        var tokenSource2 = new CancellationTokenSource();
        CancellationToken ct = tokenSource2.Token;

        var threads = nics.Select(n => Task.Run(() => n.computer.RunProgramAsync(ct))).ToArray();

        while(true){
            // Console.WriteLine(nics.Where(n => n.reading).Select(n=>n.address).Aggregate("", (a,b) => a+b+","));
            if (nics.Where(n => n.reading).Count() == 0)
            {
                if (SeenYs.Contains(NAT.Y))
                {
                    Console.WriteLine("Double sent: " + NAT.Y);
                    break;
                }
                SeenYs.Add(NAT.Y);
                Console.WriteLine($"NAT sent: {NAT.X} {NAT.Y}");
                channels[0].Writer.TryWrite((NAT.X, NAT.Y));
            }
            Thread.Sleep(100);
        } 
        tokenSource2.Cancel();
        try {
            Task.WaitAll(threads);
        }
        catch {}
        Console.WriteLine("Done with part 2: " + NAT.Y);
    }
}