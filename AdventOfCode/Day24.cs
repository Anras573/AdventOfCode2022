using System.Numerics;

namespace AdventOfCode;

public class Day24 : BaseDay
{
    private readonly string[] _input;
    private readonly Vector2 _bounds;

    private static readonly Dictionary<char, BlizzardType> BlizzardTypeDictionary = new()
    {
        { '^', BlizzardType.Up },
        { '>', BlizzardType.Right },
        { 'v', BlizzardType.Down },
        { '<', BlizzardType.Left },
    };
    
    private static readonly Vector2[] PossibleMoves = {
        new(1, 0),
        new(-1, 0),
        new(0, 0),
        new(0, 1),
        new(0, -1)
    };
    
    private static readonly int[] HorizontalMovementAxes = { 0, 1, 2, 3, 4 };
    private static readonly int[] VerticalMovementAxes = { 1, 0, 2, 4, 3 };

    public Day24()
    {
        _input = File.ReadAllLines(InputFilePath);
        _bounds = new Vector2(_input[0].Length - 2, _input.Length - 2);
    }
    
    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part1()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part2()}");

    private int Part1()
    {
        var storms = Parse();

        var currentPosition = new Vector2(0, -1);
        var targetPosition = _bounds with { X = _bounds.X - 1 };
        
        var fastest = Trace(currentPosition, targetPosition, storms, 1);

        return fastest;
    }

    private int Part2()
    {
        var storms = Parse();

        var currentPosition = new Vector2(0, -1);
        var targetPosition = _bounds with { X = _bounds.X - 1 };
        
        var fastest = Trace(currentPosition, targetPosition, storms, 1);
        fastest = Trace(targetPosition, currentPosition, storms, fastest + 1);
        fastest = Trace(currentPosition, targetPosition, storms, fastest + 1);

        return fastest;
    }
    
    private List<Storms> Parse()
    {
        var blizzards = new List<Blizzard>();
        var positions = new HashSet<Vector2>();

        for (var y = 0; y < _input.Length; y++)
        {
            var line = _input[y];
            for (var x = 0; x < line.Length; x++)
            {
                if (line[x] == '.' || line[x] == '#') continue;

                var position = new Vector2(x - 1, y - 1);

                blizzards.Add(new Blizzard(position, BlizzardTypeDictionary[line[x]]));

                positions.Add(position);
            }
        }
        
        return new List<Storms> { new(blizzards, positions) };
    }

    private int Trace(Vector2 currentPosition, Vector2 targetPosition, List<Storms> storms, int step)
    {
        var queue = new PriorityQueue<QueueElement, int>();
        queue.Enqueue(new QueueElement(currentPosition, step), step);

        var visited = new HashSet<QueueElement>();

        var fastest = int.MaxValue;

        while (queue.Count > 0)
        {
            var (position, currentStep) = queue.Dequeue();
            
            if (currentStep >= fastest) continue;
            
            while (currentStep >= storms.Count)
            {
                var nextBlizzards = storms[^1].Blizzards.Select(b => b with { Position = Move(b) }).ToList();
                var storm = new Storms(nextBlizzards, nextBlizzards.Select(b => b.Position).ToHashSet());
                storms.Add(storm);
            }

            var currentStorm = storms[currentStep];

            var delta = Vector2.Abs(targetPosition - position);

            foreach (var i in delta.X > delta.Y ? HorizontalMovementAxes : VerticalMovementAxes)
            {
                var testPosition = position + PossibleMoves[i];

                if (testPosition == targetPosition)
                {
                    fastest = Math.Min(fastest, currentStep);
                    break;
                }

                if (testPosition != position
                    && (testPosition.X < 0
                        || testPosition.Y < 0
                        || testPosition.X >= _bounds.X
                        || testPosition.Y >= _bounds.Y))
                    continue;
                
                if (currentStorm.Positions.Contains(testPosition))
                    continue;

                var queueElement = new QueueElement(testPosition, currentStep + 1);
                
                if (visited.Contains(queueElement))
                    continue;

                visited.Add(queueElement);
                queue.Enqueue(queueElement, queueElement.CurrentStep);
            }
        }
        
        return fastest;
    }

    private Vector2 Move(Blizzard blizzard)
    {
        Vector2 WrapTo(Vector2 position) =>
            new((position.X + _bounds.X) % _bounds.X, (position.Y + _bounds.Y) % _bounds.Y);
        
        return blizzard.Type switch
        {
            BlizzardType.Right => WrapTo(blizzard.Position + new Vector2( 1,  0)),
            BlizzardType.Left  => WrapTo(blizzard.Position + new Vector2(-1,  0)),
            BlizzardType.Down  => WrapTo(blizzard.Position + new Vector2( 0,  1)),
            BlizzardType.Up    => WrapTo(blizzard.Position + new Vector2( 0, -1)),
            _ => throw new ArgumentException($"Unknown BlizzardType: {blizzard.Type}", nameof(blizzard))
        };
    }

    private enum BlizzardType { Up, Right, Down, Left }
    private record struct Blizzard(Vector2 Position, BlizzardType Type);
    private record struct QueueElement(Vector2 Position, int CurrentStep);
    private record Storms(List<Blizzard> Blizzards, HashSet<Vector2> Positions);
}