using System.Text.Json;

namespace AdventOfCode;

public class Day13 : BaseDay
{
    private readonly string[] _input;

    public Day13()
    {
        _input = File.ReadAllLines(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part1()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part2()}");

    private int Part1()
    {
        var sum = 0;

        var filteredInput = ParseInput();

        var pairIndex = 1;
        
        for (var i = 0; i < filteredInput.Count; i += 2)
        {
            var left = filteredInput[i];
            var right = filteredInput[i + 1];

            if (Compare(left, right) < 0)
            {
                sum += pairIndex;
            }

            pairIndex++;
        }
        
        return sum;
    }
    
    private int Part2()
    {
        var filteredInput = ParseInput();

        var data1 = Parse("[[2]]");
        var data2 = Parse("[[6]]");
        
        filteredInput.Add(data1);
        filteredInput.Add(data2);

        filteredInput.Sort(Compare);

        var sum = (filteredInput.IndexOf(data1) + 1) * (filteredInput.IndexOf(data2) + 1);

        return sum;
    }

    private List<Data> ParseInput()
    {
        return _input
            .Where(i => !string.IsNullOrWhiteSpace(i))
            .Select(Parse)
            .ToList();
    }

    private static Data Parse(string value)
    {
        var element = JsonSerializer.Deserialize<JsonElement>(value);

        return Parse(element);
    }

    private static Data Parse(JsonElement element)
    {
        return element.ValueKind == JsonValueKind.Number
            ? new Number(element.GetInt32())
            : new Array(element.EnumerateArray().Select(Parse).ToList());
    }

    private int Compare(Data left, Data right)
    {
        return (left, right) switch
        {
            (not null, null) => +1,
            (null, not null) => -1,
            (Number l, Number r) => Comparer<int>.Default.Compare(l.Value, r.Value),
            (Number l, Array r) => Compare(new Array(new() { l }), r),
            (Array l, Number r) => Compare(l, new Array(new() { r })),
            (Array l, Array r) => ZipLongest(l.Data, r.Data, Compare).SkipWhile(x => x == 0).FirstOrDefault(),
            _ => throw new ArgumentOutOfRangeException(nameof(left))
        };
    }

    private static IEnumerable<int> ZipLongest(List<Data> left, List<Data> right, Func<Data, Data, int> selector)
    {
        using var leftEnumerator = left.GetEnumerator();
        using var rightEnumerator = right.GetEnumerator();
        
        var hasLeft = leftEnumerator.MoveNext();
        var hasRight = rightEnumerator.MoveNext();

        while (hasLeft || hasRight)
        {
            yield return (hasLeft, hasRight) switch
            {
                (true, true) => selector(leftEnumerator.Current, rightEnumerator.Current),
                (true, false) => selector(leftEnumerator.Current, default),
                (false, true) => selector(default, rightEnumerator.Current),
                (false, false) => throw new ArgumentException(nameof(hasLeft))
            };
            
            hasLeft = leftEnumerator.MoveNext();
            hasRight = rightEnumerator.MoveNext();
        }
    }
}

public abstract record Data;
public record Number(int Value) : Data;
public record Array(List<Data> Data) : Data;
