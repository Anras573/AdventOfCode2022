namespace AdventOfCode;

public class Day04 : BaseDay
{
    private readonly string[] _input;

    public Day04()
    {
        _input = File.ReadAllLines(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part1()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part2()}");

    private int Part1()
    {
        var sum = 0;

        foreach (var line in _input)
        {
            var pair = line.Split(',');
            var p1 = pair[0].Split('-').Select(int.Parse).ToList();
            var p2 = pair[1].Split('-').Select(int.Parse).ToList();

            if ((p1[0] >= p2[0] && p1[0] <= p2[1] && p1[1] >= p2[0] && p1[1] <= p2[1])
                || (p2[0] >= p1[0] && p2[0] <= p1[1] && p2[1] >= p1[0] && p2[1] <= p1[1]))
                sum++;
        }

        return sum;
    }
    
    private int Part2()
    {
        var sum = 0;
        
        foreach (var line in _input)
        {
            var pair = line.Split(',');
            var p1 = pair[0].Split('-').Select(int.Parse).ToList();
            var p2 = pair[1].Split('-').Select(int.Parse).ToList();
            
            if ((p1[0] >= p2[0] && p1[0] <= p2[1]) || (p1[1] >= p2[0] && p1[1] <= p2[1]) 
                || (p2[0] >= p1[0] && p2[0] <= p1[1]) || (p2[1] >= p1[0] && p2[1] <= p1[1]))
                sum++;
        }

        return sum;
    }
}