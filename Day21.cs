using NUnit.Framework;
namespace AoC2019;

class Day21 : Puzzle
{

    public class SpringDroid
    {
        IntcodeComputer computer;

        public SpringDroid(string code)
        {
            computer = new IntcodeComputer(lines[0])
            {
                inputs = Output().GetEnumerator(),
                output = Input
            };
            this.code = code;
        }

        public void Run() => computer.RunProgram();

        public string code = "";

        public IEnumerable<long> Output()
        {
            for (int x =0; x<code.Length; x++) { yield return (long)code[x]; }
        }

        public void Input(long i)
        {
            if (i>255 || i < 0) Console.WriteLine(i);
            else
            {
                Console.Write((char)i);
            }
        }
    }

    public override void Part1()
    {
        new SpringDroid("NOT A T\nOR T J\nNOT B T\nOR T J\nNOT C T\nOR T J\nAND D J\nWALK\n").Run();
    }

    public override void Part2()
    {
        new SpringDroid("NOT A T\nOR T J\nNOT B T\nOR T J\nNOT C T\nOR T J\nOR J T\nAND D J\nAND T J\nNOT J T\nOR H T\nOR E T\nAND T J\nRUN\n").Run();
    }
}