using NUnit.Framework;
namespace AoC2019;

class IntcodeComputer
{
    // (1,a,b,c) = a+b stored in c
    // (2,a,b,c) = a*b stored in c
    public IntcodeComputer()
    {
    }

    public IntcodeComputer(string initial) : base()
    {
        if (!String.IsNullOrEmpty(initial))
        {
            ParseCode(initial);
        }
    }
    public Dictionary<long, long> Memory = new Dictionary<long, long>();

    public long this[long location]
    {
        get
        {
            long result = 0L;
            Memory.TryGetValue(location, out result);
            return result;
        }

        set
        {
            Memory[location] = value;
        }
    }
    private System.Text.StringBuilder sb = new System.Text.StringBuilder();

    public long Count() => Memory.Keys.Max() + 1;
    public override string ToString()
    {
        long count = Count();
        sb.Clear();
        for (long i = 0; i < count; i++)
        {
            if (i > 0) sb.Append(',');
            sb.Append(this[i]);
        }
        return sb.ToString();
    }

    public int index = 0;

    public void ParseCode(string code)
    {
        Memory.Clear();
        foreach (int mem in code.Split(',').Select(value => Convert.ToInt64(value)))
        {
            this[index++] = mem;
        }
    }

    public IntcodeComputer RunProgram(string code)
    {
        ParseCode(code);
        RunProgram();
        return this;
    }

    public IEnumerator<long>? inputs;

    public IAsyncEnumerator<long>? inputsAsync;


    public Action<long>? output;

    [TestCase(1002, 0, ExpectedResult = 0)]
    [TestCase(1002, 1, ExpectedResult = 1)]
    [TestCase(1002, 2, ExpectedResult = 0)]
    public static int ParamMode(long opcode, long position)
    {
        opcode /= 100;
        while (position-- > 0)
        {
            opcode /= 10;
        }
        return (int)(opcode % 10);
    }

    public long ReadParam(long position)
    {
        switch (ParamMode(this[ip], position))
        {
            case 0: return this[(long)this[ip + position + 1]];
            case 1: return this[ip + position + 1];
            case 2: return this[(long)this[ip + position + 1] + relative];
            default: throw new Exception("Unimplemented param mode");
        }
    }
    public void WriteParam(long position, long value)
    {
        switch (ParamMode(this[ip], position))
        {
            case 0: this[(long)this[ip + position + 1]] = value; return;
            case 2: this[(long)this[ip + position + 1] + relative] = value; return;
            default: throw new Exception("Unimplemented param mode");
        }
    }

    public long ip = 0;
    public long relative = 0;
    public async Task<long> StepAsync()
    {
        int opcode = (int)this[ip] % 100;
        if (opcode == 1) // ADD
        {
            WriteParam(2, ReadParam(0) + ReadParam(1));
            return ip += 4;
        }
        if (opcode == 2) // MUL
        {
            WriteParam(2, ReadParam(0) * ReadParam(1));
            return ip += 4;
        }
        if (opcode == 3)
        {
            long value = 0;
            if (inputs != null)
            {
                if (!inputs.MoveNext())
                {
                    throw new Exception("Ran out of input");
                }
                value = inputs.Current;
            }
            else if (inputsAsync != null)
            {
                if (!(await inputsAsync.MoveNextAsync()))
                {
                    throw new Exception("Ran out of input");
                }
                value = inputsAsync.Current;
            }
            else
            {
                throw new Exception("Ran out of input");
            }
            WriteParam(0, value);
            return ip += 2;
        }
        if (opcode == 4)
        {
            if (output is null) throw new Exception("Output not piped");
            output(ReadParam(0));
            return ip += 2;
        }
        if (opcode == 5)
        {
            if (ReadParam(0) != 0)
            {
                return ip = ReadParam(1);
            }
            return ip += 3;
        }
        if (opcode == 6)
        {
            if (ReadParam(0) == 0)
            {
                return ip = ReadParam(1);
            }
            return ip += 3;
        }

        if (opcode == 7) // LT
        {
            WriteParam(2, ReadParam(0) < ReadParam(1) ? 1 : 0);
            return ip += 4;
        }

        if (opcode == 8) // EQ
        {
            WriteParam(2, ReadParam(0) == ReadParam(1) ? 1 : 0);
            return ip += 4;
        }

        if (opcode == 9)
        {
            relative += ReadParam(0);
            return ip += 2;
        }

        if (opcode == 99) // HALT
        {
            // Console.WriteLine("HALT");
            return -1;
        }

        throw new Exception("Unknown opcode: " + opcode);
    }

    public IntcodeComputer RunProgram()
    {
        return RunProgramAsync().Result;
    }

    public async Task<IntcodeComputer> RunProgramAsync()
    {
        index = 0;
        while ((await StepAsync().ConfigureAwait(false)) >= 0) ;
        return this;
    }

    [TestCase("1,0,0,0,99", ExpectedResult = "2,0,0,0,99")]
    [TestCase("1,1,1,4,99,5,6,0,99", ExpectedResult = "30,1,1,4,2,5,6,0,99")]
    [TestCase("1,9,10,3,2,3,11,0,99,30,40,50", ExpectedResult = "3500,9,10,70,2,3,11,0,99,30,40,50")]
    [TestCase("1101,100,-1,4,0", ExpectedResult="1101,100,-1,4,99")]
    public static string TestCase(string input)
    {
        return new IntcodeComputer(input).RunProgram().ToString();
    }

    [TestCase(ExpectedResult = 25)]
    public static long InputTest()
    {
        long result = 0;
        void SetResult(long v) => result = v;
        var computer = new IntcodeComputer("3,0,4,0,99")
        {
            inputs = (new List<long> {25}).GetEnumerator(),
            output = SetResult
        };
        computer.RunProgram();
        return result;
    }

    [TestCase(0, ExpectedResult = 999)]
    [TestCase(8, ExpectedResult = 1000)]
    [TestCase(9, ExpectedResult = 1001)]
    public static long JumpTests(int input)
    {        
        long result = 0;
        void SetResult(long v) => result = v;
        var computer = new IntcodeComputer("3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99")
        {
            inputs = (new List<long> {input}).GetEnumerator(),
            output = SetResult
        };
        computer.RunProgram();
        return result;

    }

}