namespace AdventOfCode;

public class Day03 : BaseDay
{
    private readonly string[] _input;

    public Day03()
    {
        _input = File.ReadAllLines(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {SumOfItemType()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {SumOfBadges()}");

    private int SumOfItemType()
    {
        var sum = 0;

        foreach (var line in _input)
        {
            var half = line.Length / 2;
            var left = 0;
            var right = half;
            var leftLetters = new HashSet<char>();
            var rightLetters = new HashSet<char>();

            while (left < half)
            {
                leftLetters.Add(line[left]);
                rightLetters.Add(line[right]);
                
                if (rightLetters.Contains(line[left]))
                {
                    sum += ConvertFromCharToInt(line[left]);
                    break;
                }
                
                if (leftLetters.Contains(line[right]))
                {
                    sum += ConvertFromCharToInt(line[right]);
                    break;
                }
                
                left++;
                right++;
            }
        }

        return sum;
    }

    private int SumOfBadges()
    {
        var sum = 0;

        for (var i = 0; i < _input.Length; i += 3)
        {
            var hashSet1 = _input[i].ToCharArray().ToHashSet();
            var hashSet2 = _input[i + 1].ToCharArray().ToHashSet();
            var hashSet3 = _input[i + 2].ToCharArray().ToHashSet();

            var badge = hashSet1.First(c => hashSet2.Contains(c) && hashSet3.Contains(c));
            sum += ConvertFromCharToInt(badge);
        }

        return sum;
    }
    
    private static int ConvertFromCharToInt(char c)
    {
        return c > 97 ? c - 96 : c - 38;
    }
}