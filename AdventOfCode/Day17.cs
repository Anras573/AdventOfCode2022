namespace AdventOfCode;

public class Day17 : BaseDay
{
    private readonly string[] _input;

    private readonly List<string[]> _rocks = new()
    {
        new[] { "####" },
        new[] { " # ", "###", " # " },
        new[] { "  #", "  #", "###" },
        new[] { "#", "#", "#", "#" },
        new[] { "##", "##" }
    };

    public Day17()
    {
        _input = File.ReadAllLines(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part1()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part2()}");

    private long Part1()
    {
        const long rounds = 2022;
        
        return Simulate(rounds);
    }
    
    private long Part2()
    {
        const long rounds = 1_000_000_000_000;
        
        return Simulate(rounds);
    }

    private long Simulate(long rounds)
    {
        var rows = new List<string> { "+-------+" };
        
        var jetStreams = _input.First().ToCharArray().ToList();

        using var rockLoop = EternalLoop(_rocks).GetEnumerator();
        using var jetStreamLoop = EternalLoop(jetStreams).GetEnumerator();
        
        long rowsNotStored = 0;
        var rockCache = new Dictionary<string, (long rocks, long height)>();
        while (rounds > 0)
        {
            var hash = string.Join(Environment.NewLine, rows);
            if (rockCache.TryGetValue(hash, out var cache))
            {
                var height = CalcHeight() - cache.height;
                var length = cache.rocks - rounds;

                rowsNotStored += (rounds / length) * height;
                rounds %= length;
                break;
            }

            rockCache[hash] = (rounds, CalcHeight());
            rounds--;
            rowsNotStored += AddRock(rockLoop, jetStreamLoop, rows);
        }

        while (rounds > 0)
        {
            rowsNotStored += AddRock(rockLoop, jetStreamLoop, rows);
            rounds--;
        }
        
        long CalcHeight() => rows.Count + rowsNotStored - 1;

        return CalcHeight();
    }

    private int AddRock(IEnumerator<string[]> rockLoop, IEnumerator<char> jetStreamLoop, List<string> rows)
    {
        rockLoop.MoveNext();
        var rock = rockLoop.Current;

        for (var i = 0; i < rock?.Length + 3; i++)
        {
            rows.Insert(0, "|       |");
        }

        var x = 3;
        var y = 0;

        while (true)
        {
            jetStreamLoop.MoveNext();
            var jet = jetStreamLoop.Current;
            switch (jet)
            {
                case '>' when !Hit(rock, rows, x + 1, y):
                    x++;
                    break;
                case '<' when !Hit(rock, rows, x - 1, y):
                    x--;
                    break;
            }

            if (Hit(rock, rows, x, y + 1))
                break;

            y++;
        }

        return AddRockToRows(rock, rows, x, y);
    }
    
    private static bool Hit(string[] rock, List<string> rows, int x, int y)
    {
        var rowLength = rock.Length;
        var colLength = rock[0].Length;
            
        for (var row = 0; row < rowLength; row++) {
            for (var col = 0; col < colLength; col++) {
                if (rock[row][col] == '#' && rows[row + y][col + x] != ' ') {
                    return true;
                }
            }
        }
        return false;
    }
    
    private int AddRockToRows(string[] rock, List<string> rows, int x, int y)
    {
        var rowLength = rock.Length;
        var colLenght = rock[0].Length;
            
        for (var row = 0; row < rowLength; row++) {
            var chars = rows[row + y].ToArray();
            
            for (var col = 0; col < colLenght; col++) {
                if (rock[row][col] == '#') {
                    chars[col + x] = '#';
                }
            }
            
            rows[row + y] = string.Join(string.Empty, chars);
        }

        while (!rows[0].Contains('#')) {
            rows.RemoveAt(0);
        }

        if (rows.Count <= 75) return 0;
        
        var r = rows.Count - 75;
        rows.RemoveRange(75, r);
        
        return r;

    }

    private static IEnumerable<T> EternalLoop<T>(IEnumerable<T> items)
    {
        while (true)
        {
            foreach (var item in items)
            {
                yield return item;
            }
        }
    }
}