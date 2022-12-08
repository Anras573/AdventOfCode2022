using Spectre.Console.Rendering;

namespace AdventOfCode;

public class Day08 : BaseDay
{
    private readonly string[] _input;

    public Day08()
    {
        _input = File.ReadAllLines(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part1()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part2()}");

    private int Part1()
    {
        var sum = 0;
        var forest = ParseInput();

        for (var y = 0; y < forest.Length; y++)
        {
            for (var x = 0; x < forest[y].Length; x++)
            {
                if (y == 0 || x == 0 || y == forest.Length - 1 || x == forest[y].Length - 1)
                {
                    sum++;
                    continue;
                }
                
                var dY = y;
                var maxHeight = forest[y][x] - 1;
                var isVisible = true;
                
                while (dY != 0 && isVisible)
                {
                    dY--;
                    if (forest[dY][x] > maxHeight)
                        isVisible = false;
                }

                if (isVisible)
                {
                    sum++;
                    continue;
                }
                
                dY = y;
                isVisible = true;
                
                while (dY != forest.Length - 1 && isVisible)
                {
                    dY++;
                    if (forest[dY][x] > maxHeight)
                        isVisible = false;
                }
                
                if (isVisible)
                {
                    sum++;
                    continue;
                }
                
                var dX = x;
                isVisible = true;

                while (dX != 0 && isVisible)
                {
                    dX--;
                    if (forest[y][dX] > maxHeight)
                        isVisible = false;
                }

                if (isVisible)
                {
                    sum++;
                    continue;
                }
                
                dX = x;
                isVisible = true;
                
                while (dX != forest[y].Length - 1 && isVisible)
                {
                    dX++;
                    if (forest[y][dX] > maxHeight)
                        isVisible = false;
                }
                
                if (isVisible)
                    sum++;
            }
        }
        
        return sum;
    }
    
    private int Part2()
    {
        var maxViewingDistance = 0;
        var forest = ParseInput();

        for (var y = 0; y < forest.Length; y++)
        {
            for (var x = 0; x < forest[y].Length; x++)
            {
                var (v1, v2, v3, v4) = (0, 0, 0, 0);

                var dY = y;
                var maxHeight = forest[y][x] - 1;
                var isVisible = true;
                
                while (dY != 0 && isVisible)
                {
                    dY--;
                    v1++;
                    if (forest[dY][x] > maxHeight)
                        isVisible = false;
                }

                dY = y;
                isVisible = true;
                
                while (dY != forest.Length - 1 && isVisible)
                {
                    dY++;
                    v2++;
                    if (forest[dY][x] > maxHeight)
                        isVisible = false;
                }

                var dX = x;
                isVisible = true;

                while (dX != 0 && isVisible)
                {
                    dX--;
                    v3++;
                    if (forest[y][dX] > maxHeight)
                        isVisible = false;
                }

                dX = x;
                isVisible = true;
                
                while (dX != forest[y].Length - 1 && isVisible)
                {
                    dX++;
                    v4++;
                    if (forest[y][dX] > maxHeight)
                        isVisible = false;
                }

                var viewDistance = v1 * v2 * v3 * v4;
                maxViewingDistance = Math.Max(viewDistance, maxViewingDistance);
            }
        }
        
        return maxViewingDistance;
    }
    
    private int[][] ParseInput()
    {
        var forest = new int[_input.Length][];

        for (var i = 0; i < _input.Length; i++)
        {
            forest[i] = _input[i].Select(c => int.Parse(c.ToString())).ToArray();
        }

        return forest;
    }
}