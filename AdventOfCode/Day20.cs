namespace AdventOfCode;

public class Day20 : BaseDay
{
    private readonly string[] _input;

    public Day20()
    {
        _input = File.ReadAllLines(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part1()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part2()}");

    private long Part1()
    {
        var coordinates = _input
            .Select((value, index) => Coordinate.Parse(value, index))
            .ToList();

        return Decrypt(coordinates);
    }
    
    private long Part2()
    {
        var coordinates = _input
            .Select((value, index) => Coordinate.Parse(value, index, 811589153))
            .ToList();

        return Decrypt(coordinates, 10);
    }

    private static long Decrypt(List<Coordinate> coordinates, int decryptionCount = 1)
    {
        var mixin = new List<Coordinate>(coordinates);

        var length = coordinates.Count - 1;

        foreach (var _ in Enumerable.Range(0, decryptionCount))
        {
            foreach (var coordinate in coordinates)
            {
                var index = mixin.IndexOf(coordinate);
                var newIndex = (int)((index + coordinate.Value) % length);

                while (newIndex < 0)
                {
                    newIndex += length;
                }

                mixin.RemoveAt(index);
                mixin.Insert(newIndex, coordinate);
            }
        }

        var indexOfZero = mixin.FindIndex(l => l.Value == 0);

        return mixin[(indexOfZero + 1000) % mixin.Count].Value
               + mixin[(indexOfZero + 2000) % mixin.Count].Value
               + mixin[(indexOfZero + 3000) % mixin.Count].Value;
    }

    private record struct Coordinate(int Index, long Value)
    {
        public static Coordinate Parse(string line, int index, int decryptionKey = 1)
        {
            return new Coordinate(index, long.Parse(line) * decryptionKey);
        }
    };
}