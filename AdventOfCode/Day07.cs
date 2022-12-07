namespace AdventOfCode;

public class Day07 : BaseDay
{
    private readonly string[] _input;

    public Day07()
    {
        _input = File.ReadAllLines(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part1()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part2()}");

    private int Part1()
    {
        const int maxSize = 100_000;

        var diskSpacePerDirectory = GetFileSystemInfo();
        
        var answer = diskSpacePerDirectory.Values.Where(i => i <= maxSize).Sum();

        return answer;
    }

    private int Part2()
    {
        var diskSpacePerDirectory = GetFileSystemInfo();
        
        var maxSize = 30_000_000 - (70_000_000 - diskSpacePerDirectory["/"]);
        var answer = diskSpacePerDirectory.Values.OrderBy(i => i).First(i => i >= maxSize);

        return answer;
    }

    private Dictionary<string, int> GetFileSystemInfo()
    {
        var dirHistory = new Stack<string>();
        var diskSpacePerDirectory = new Dictionary<string, int>();
        
        foreach (var line in _input)
        {
            var output = line.Split(" ");
            
            if (output[0] == "$" && output[1] == "cd")
            {
                if (output[2] == "..")
                    dirHistory.Pop();
                else
                    dirHistory.Push(output[2]);
            }
            else if (char.IsDigit(line[0]))
            {
                var size = int.Parse(output[0]);
                
                for (var i = 0; i < dirHistory.Count; i++)
                {
                    var dir = string.Join("/", dirHistory.Reverse().Take(i + 1));

                    if (diskSpacePerDirectory.ContainsKey(dir))
                        diskSpacePerDirectory[dir] += size;
                    else
                        diskSpacePerDirectory[dir] = size;
                }
            }
        }

        return diskSpacePerDirectory;
    }
}