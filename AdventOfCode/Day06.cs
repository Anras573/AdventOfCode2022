namespace AdventOfCode;

public class Day06 : BaseDay
{
    private readonly string[] _input;

    public Day06()
    {
        _input = File.ReadAllLines(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part1()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part2()}");

    private int Part1()
    {
        return Solve(4);
    }

    private int Part2()
    {
        return Solve(14);
    }
    
    private int Solve(int limit)
    {
        var endOfMarker = 0;
        var queue = new Queue<char>();
        var line = _input[0];

        while (endOfMarker <= line.Length)
        {
            if (queue.Contains(line[endOfMarker]))
            {
                while (queue.Dequeue() != line[endOfMarker]) { }
            }
            
            queue.Enqueue(line[endOfMarker]);
            endOfMarker++;
            
            if (queue.Count == limit)
                return endOfMarker;
        }
        
        return endOfMarker;
    }
}