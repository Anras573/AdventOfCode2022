namespace AdventOfCode;

public class Day18 : BaseDay
{
    private readonly string[] _input;

    public Day18()
    {
        _input = File.ReadAllLines(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part1()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part2()}");

    private int Part1()
    {
        var surfaceArea = 0;
        var cubes = _input.Select(Position.Parse).ToList();
        var surfaceAreaContainer = cubes.ToHashSet();

        foreach (var cube in cubes) 
        {
            if (!surfaceAreaContainer.Contains(cube with { X = cube.X + 1 })) surfaceArea++;
            if (!surfaceAreaContainer.Contains(cube with { X = cube.X - 1 })) surfaceArea++;
            if (!surfaceAreaContainer.Contains(cube with { Y = cube.Y + 1 })) surfaceArea++;
            if (!surfaceAreaContainer.Contains(cube with { Y = cube.Y - 1 })) surfaceArea++;
            if (!surfaceAreaContainer.Contains(cube with { Z = cube.Z + 1 })) surfaceArea++;
            if (!surfaceAreaContainer.Contains(cube with { Z = cube.Z - 1 })) surfaceArea++;
        }
        
        return surfaceArea;
    }
    
    private int Part2()
    {
        var surfaceArea = 0;
        var cubes = _input.Select(Position.Parse).ToList();
        var surfaceAreaContainer = cubes.ToHashSet();

        var max = new Position
        {
            X = surfaceAreaContainer.Max(c => c.X),
            Y = surfaceAreaContainer.Max(c => c.Y),
            Z = surfaceAreaContainer.Max(c => c.Z)
        };
        
        var min = new Position
        {
            X = surfaceAreaContainer.Min(c => c.X),
            Y = surfaceAreaContainer.Min(c => c.Y),
            Z = surfaceAreaContainer.Min(c => c.Z)
        };

        var rangeX = Enumerable.Range(min.X, max.X + 1).ToHashSet();
        var rangeY = Enumerable.Range(min.Y, max.Y + 1).ToHashSet();
        var rangeZ = Enumerable.Range(min.Z,  max.Z + 1).ToHashSet();

        bool IsInWater(Position cube)
        {
            if (surfaceAreaContainer.Contains(cube)) return false;
            
            var checkedCubes = new HashSet<Position>();
            var cubesToCheck = new Queue<Position>();
            cubesToCheck.Enqueue(cube);
            while (cubesToCheck.Any())
            {
                var tmpCube = cubesToCheck.Dequeue();
                
                if (checkedCubes.Contains(tmpCube)) continue;

                checkedCubes.Add(tmpCube);

                if (!rangeX.Contains(tmpCube.X) || !rangeY.Contains(tmpCube.Y) || !rangeZ.Contains(tmpCube.Z))
                    return true;

                if (surfaceAreaContainer.Contains(tmpCube)) continue;
                
                cubesToCheck.Enqueue(tmpCube with { X = tmpCube.X + 1 });
                cubesToCheck.Enqueue(tmpCube with { X = tmpCube.X - 1 });
                cubesToCheck.Enqueue(tmpCube with { Y = tmpCube.Y + 1 });
                cubesToCheck.Enqueue(tmpCube with { Y = tmpCube.Y - 1 });
                cubesToCheck.Enqueue(tmpCube with { Z = tmpCube.Z + 1 });
                cubesToCheck.Enqueue(tmpCube with { Z = tmpCube.Z - 1 });
            }
            
            return false;
        }

        foreach (var cube in cubes) 
        {
            if (IsInWater(cube with { X = cube.X + 1 })) surfaceArea++;
            if (IsInWater(cube with { X = cube.X - 1 })) surfaceArea++;
            if (IsInWater(cube with { Y = cube.Y + 1 })) surfaceArea++;
            if (IsInWater(cube with { Y = cube.Y - 1 })) surfaceArea++;
            if (IsInWater(cube with { Z = cube.Z + 1 })) surfaceArea++;
            if (IsInWater(cube with { Z = cube.Z - 1 })) surfaceArea++;
        }
        
        return surfaceArea;
    }

    private record struct Position(int X, int Y, int Z)
    {
        public static Position Parse(string input)
        {
            var split = input.Split(',');
            return new Position(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]));
        }
    };
}