namespace AdventOfCode;

public class Day05 : BaseDay
{
    private readonly string[] _input;

    public Day05()
    {
        _input = File.ReadAllLines(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part1()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part2()}");

    private string Part1()
    {
        var stacks = new Stack<char>[]
        {
            new Stack<char>(),
            new Stack<char>(),
            new Stack<char>(),
            new Stack<char>(),
            new Stack<char>(),
            new Stack<char>(),
            new Stack<char>(),
            new Stack<char>(),
            new Stack<char>()
        };

        var reverseStacks = true;

        foreach (var line in _input)
        {
            if (line.Contains('['))
            {
                var i = 1;
                var j = 0;
                while (i < line.Length)
                {
                    if (char.IsLetter(line[i]))
                        stacks[j].Push(line[i]);
                    
                    i += 4;
                    j++;
                }
            }
            
            if (line.StartsWith("move"))
            {
                if (reverseStacks)
                {
                    for (var i = 0; i < stacks.Length; i++)
                    {
                        var tmpStack = new Stack<char>();
                        var stack = stacks[i];
                        
                        while (stack.Count != 0)
                            tmpStack.Push(stack.Pop());
                        
                        stacks[i] = tmpStack;
                    }

                    reverseStacks = false;
                }

                var instruction = line.Split(" ");
                var stackFrom = stacks[int.Parse(instruction[3]) - 1];
                var stackTo = stacks[int.Parse(instruction[5]) - 1];
                
                for (var i = 0; i < int.Parse(instruction[1]); i++)
                    stackTo.Push(stackFrom.Pop());
            }
        }

        return string.Join("", stacks.Select(s => s.Peek()));
    }
    
    private string Part2()
    {
        var stacks = new Stack<char>[]
        {
            new Stack<char>(),
            new Stack<char>(),
            new Stack<char>(),
            new Stack<char>(),
            new Stack<char>(),
            new Stack<char>(),
            new Stack<char>(),
            new Stack<char>(),
            new Stack<char>()
        };
        
        var tmpStack = new Stack<char>();

        var reverseStacks = true;

        foreach (var line in _input)
        {
            if (line.Contains('['))
            {
                var i = 1;
                var j = 0;
                while (i < line.Length)
                {
                    if (char.IsLetter(line[i]))
                        stacks[j].Push(line[i]);
                    
                    i += 4;
                    j++;
                }
            }
            
            if (line.StartsWith("move"))
            {
                if (reverseStacks)
                {
                    for (var i = 0; i < stacks.Length; i++)
                    {
                        tmpStack = new Stack<char>();
                        var stack = stacks[i];
                        
                        while (stack.Count != 0)
                            tmpStack.Push(stack.Pop());
                        
                        stacks[i] = tmpStack;
                    }

                    reverseStacks = false;
                }

                var instruction = line.Split(" ");
                var stackFrom = stacks[int.Parse(instruction[3]) - 1];
                var stackTo = stacks[int.Parse(instruction[5]) - 1];

                tmpStack = new Stack<char>();
                
                for (var i = 0; i < int.Parse(instruction[1]); i++)
                {
                    tmpStack.Push(stackFrom.Pop());
                }
                
                for (var i = 0; i < int.Parse(instruction[1]); i++)
                {
                    stackTo.Push(tmpStack.Pop());
                }
            }
        }

        return string.Join("", stacks.Select(s => s.Peek()));
    }
}