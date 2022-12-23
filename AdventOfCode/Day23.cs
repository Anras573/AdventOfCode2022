using System.Numerics;

namespace AdventOfCode;

public class Day23 : BaseDay
{
    private readonly string[] _input;
    
    private static readonly Vector2 N = new(0, -1);
    private static readonly Vector2 S = new(0, 1);
    private static readonly Vector2 E = new(1, 0);
    private static readonly Vector2 W = new(-1, 0);
    private static readonly Vector2 NE = N + E;
    private static readonly Vector2 NW = N + W;
    private static readonly Vector2 SE = S + E;
    private static readonly Vector2 SW = S + W;

    private static readonly Vector2[] AllDirections = { NW, N, NE, E, SE, S, SW, W };

    public Day23()
    {
        _input = File.ReadAllLines(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part1()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part2()}");

    private int Part1()
    {
        var state = Parse();

        for (var i = 0; i < 10; i++)
        {
            state = Step(state);
        }

        var width = (int) state.Elves.MaxBy(e => e.X).X - (int) state.Elves.MinBy(e => e.X).X + 1;
        var height = (int) state.Elves.MaxBy(e => e.Y).Y - (int) state.Elves.MinBy(e => e.Y).Y + 1;
        
        return width * height - state.Elves.Count;
    }

    private int Part2()
    {
        var state = Parse();

        var steps = 0;

        while (state.NoChangesDetected)
        {
            state = Step(state);
            steps++;
        }

        return steps;
    }

    private State Parse()
    {
        var elves = (
                from row in Enumerable.Range(0, _input.Length)
                from col in Enumerable.Range(0, _input[0].Length)
                where _input[row][col] == '#'
                select new Vector2(col, row))
            .ToHashSet();

        var state = new State(elves, new List<Vector2> { N, S, W, E });
        
        return state;
    }

    private static State Step(State state)
    {
        bool IsTaken(Vector2 location) => state.Elves.Contains(location);
        bool IsAlone(Vector2 location) => AllDirections.All(d => !IsTaken(location + d));
        bool Suggestion(Vector2 elf, Vector2 direction) => ExtendDirection(direction).All(d => !IsTaken(elf + d));

        var suggestions = new Dictionary<Vector2, List<Vector2>>();

        foreach (var elf in state.Elves)
        {
            if (IsAlone(elf)) continue;

            foreach (var direction in state.Directions)
            {
                if (!Suggestion(elf, direction)) continue;
                    
                var location = elf + direction;

                if (!suggestions.ContainsKey(location))
                    suggestions[location] = new List<Vector2>();
                        
                suggestions[location].Add(elf);
                break;
            }
        }

        var noChangesDetected = false;
        foreach (var suggestion in suggestions)
        {
            var (to, from) = suggestion;
                
            if (from.Count != 1) continue;

            state.Elves.Remove(from.Single());
            state.Elves.Add(to);
            noChangesDetected = true;
        }

        return state with
        {
            Directions = state.Directions.Skip(1).Concat(state.Directions.Take(1)).ToList(),
            NoChangesDetected = noChangesDetected
        };
    }
    
    private static IEnumerable<Vector2> ExtendDirection(Vector2 direction)
    {
        if (direction == N) return new[] { NW, N, NE };
        if (direction == E) return new[] { NE, E, SE };
        if (direction == S) return new[] { SW, S, SE };
        if (direction == W) return new[] { NW, W, SW };

        throw new ArgumentException($"Unknown direction: {direction}", nameof(direction));
    }

    private record State(HashSet<Vector2> Elves, List<Vector2> Directions, bool NoChangesDetected = true);
}