using System.Text;

namespace AdventOfCode;

public class Day10 : BaseDay
{
    private readonly string[] _input;
    
    public Day10()
    {
        _input = File.ReadAllLines(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part1()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part2()}");

    private int Part1()
    {
        var bufferValue = 1;
        var sum = 0;
        var queue = new Queue<int>();
        var count = 1;
        
        foreach (var line in _input)
        {
            var command = line.Split(" ");
            switch (command[0])
            {
                case "noop":
                    queue.Enqueue(0);
                    break;
                case "addx":
                    queue.Enqueue(0);
                    queue.Enqueue(int.Parse(command[1]));
                    break;
                default:
                    Console.WriteLine(line);
                    break;
            }
            
            if (count == 20 || (count - 20) % 40 == 0)
                sum += count * bufferValue;

            bufferValue += queue.Dequeue();

            count++;
        }
        
        while (queue.Count != 0)
        {
            if (count == 20 || (count - 20) % 40 == 0)
                sum += count * bufferValue;

            bufferValue += queue.Dequeue();

            count++;
        }
        
        return sum;
    }
    
    private string Part2()
    {
        var crt = new char[6][];

        for (var r = 0; r < crt.Length; r++)
        {
            crt[r] = new char[40];
            for (var c = 0; c < crt[r].Length; c++)
            {
                crt[r][c] = '.';
            }
        }
        
        var bufferValue = 1;
        var queue = new Queue<int>();
        
        foreach (var line in _input)
        {
            var command = line.Split(" ");
            switch (command[0])
            {
                case "noop":
                    queue.Enqueue(0);
                    break;
                case "addx":
                    queue.Enqueue(0);
                    queue.Enqueue(int.Parse(command[1]));
                    break;
                default:
                    Console.WriteLine(line);
                    break;
            }
        }

        var row = 0;
        var column = 0;

        while (queue.Count != 0)
        {
            if (column == bufferValue -1 || column == bufferValue || column == bufferValue + 1)
            {
                crt[row][column] = '#';
            }
            
            bufferValue += queue.Dequeue();

            column++;
            if (column == 40)
            {
                column = 0;
                row++;
            }
        }

        var sb = new StringBuilder(7);
        sb.AppendLine();
        
        foreach (var line in crt)
        {
            sb.AppendLine(string.Join("", line));
        }
        
        return sb.ToString();
    }
}