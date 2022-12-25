namespace AdventOfCode;

public class Day25 : BaseDay
{
    private readonly string[] _input;

    public Day25()
    {
        _input = File.ReadAllLines(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part1()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part2()}");

    private string Part1()
    {
        var sum = _input
            .Select(ParseSnafu)
            .Sum();

        var snafu = ConvertToSnafu(sum);

        return snafu;
    }
    
    private static long ParseSnafu(string snafu)
    {
        long value = 0;

        foreach (var digit in snafu)
        {
            value *= 5;

            value += digit switch
            {
                '=' => -2,
                '-' => -1,
                '0' => 0,
                '1' => 1,
                '2' => 2,
                _ => throw new ArgumentException($"Unknown character: {digit}", nameof(snafu))
            };
        }
                
        return value;
    }

    private static string ConvertToSnafu(long sum)
    {
        var snafu = "";

        while (sum > 0)
        {
            switch (sum % 5)
            {
                case 0:
                    snafu = $"0{snafu}";
                    break;
                case 1:
                    snafu = $"1{snafu}";
                    break;
                case 2:
                    snafu = $"2{snafu}";
                    break;
                case 3:
                    snafu = $"={snafu}";
                    sum += 5;
                    break;
                case 4:
                    snafu = $"-{snafu}";
                    sum += 5;
                    break;
            }

            sum /= 5;
        }

        return snafu;
    }

    private int Part2()
    {
        return 0;
    }
}
