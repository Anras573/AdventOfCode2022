namespace AdventOfCode;

public class Day14 : BaseDay
{
    private readonly string[] _input;

    public Day14()
    {
        _input = File.ReadAllLines(InputFilePath); 
    }
    
    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part1()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part2()}");

    private int Part1()
    {
        var map = ParseMap();

        var maxY = map.Max(m => m.Y);
        
        return Solve(map, maxY);
    }
    
    private int Part2()
    {
        var map = ParseMap();

        var maxY = map.Max(m => m.Y) + 2;
        var minX = map.Min(m => m.X);
        var maxX = map.Max(m => m.X);

        for (var x = minX - maxY; x < maxX + maxY; x++)
        {
            map.Add(new Coordinate(x, maxY));
        }
        
        return Solve(map, maxY);
    }

    private static int Solve(HashSet<Coordinate> map, int maxY)
    {
        var count = 0;

        while (true)
        {
            var sandCoord = new Coordinate(500, 0);

            if (map.Contains(sandCoord)) return count;

            while (true)
            {
                if (sandCoord.Y == maxY) return count;

                var tmpSand = sandCoord with { Y = sandCoord.Y + 1 };

                if (!map.Contains(tmpSand))
                {
                    sandCoord = tmpSand;
                    continue;
                }

                tmpSand = new Coordinate(X: sandCoord.X - 1, Y: sandCoord.Y + 1);

                if (!map.Contains(tmpSand))
                {
                    sandCoord = tmpSand;
                    continue;
                }

                tmpSand = new Coordinate(X: sandCoord.X + 1, Y: sandCoord.Y + 1);

                if (!map.Contains(tmpSand))
                {
                    sandCoord = tmpSand;
                    continue;
                }

                break;
            }

            map.Add(sandCoord);
            count++;
        }
    }

    private HashSet<Coordinate> ParseMap()
    {
        var map = new HashSet<Coordinate>();

        foreach (var line in _input)
        {
            var row = line.Split(" -> ");

            for (var i = 0; i < row.Length - 1; i++)
            {
                var first = Coordinate.Parse(row[i]);
                var second = Coordinate.Parse(row[i + 1]);

                var direction = first.X == second.X
                    ? new Coordinate(0, Math.Sign(second.Y - first.Y))
                    : new Coordinate(Math.Sign(second.X - first.X), 0);

                while (first.X != second.X || first.Y != second.Y)
                {
                    map.Add(first);
                    first.X += direction.X;
                    first.Y += direction.Y;
                }

                map.Add(first);
            }
        }

        return map;
    }
    
    private record struct Coordinate(int X, int Y)
    {
        public static Coordinate Parse(string input)
        {
            var split = input.Split(',');
            return new Coordinate(int.Parse(split[0]), int.Parse(split[1]));
        }
    }
}
