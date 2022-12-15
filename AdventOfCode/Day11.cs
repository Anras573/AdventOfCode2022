namespace AdventOfCode;

public class Day11 : BaseDay
{
    private readonly string[] _input;
    
    public Day11()
    {
        _input = File.ReadAllLines(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part1()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part2()}");
    
    private int Part1()
    {
        var monkeys = ParseMonkeys().Monkeys;

        var monkeyInspections = new int[monkeys.Count];

        for (var i = 0; i < 20; i++)
        {
            foreach (var monkey in monkeys)
            {
                while (monkey.Items.Count != 0)
                {
                    var item = monkey.Items.Dequeue();
                    monkeyInspections[monkey.Number] += 1;

                    var worryLevel = monkey.Operation(item);
                    worryLevel /= 3;
                    
                    var targetMonkey = worryLevel % monkey.Modulo == 0 ? monkey.TrueMonkey : monkey.FalseMonkey;
                    monkeys[targetMonkey].Items.Enqueue(worryLevel);
                }
            }
        }

        var orderedInspections = monkeyInspections.OrderByDescending(i => i).ToArray();
        
        return orderedInspections[0] * orderedInspections[1];
    }
    
    private long Part2()
    {
        var (monkeys, modulo) = ParseMonkeys();

        var monkeyInspections = new long[monkeys.Count];
        
        for (var i = 0; i < 10000; i++)
        {
            foreach (var monkey in monkeys)
            {
                while (monkey.Items.Count != 0)
                {
                    var item = monkey.Items.Dequeue();
                    monkeyInspections[monkey.Number] += 1;

                    var worryLevel = monkey.Operation(item);

                    worryLevel %= modulo;

                    var targetMonkey = worryLevel % monkey.Modulo == 0 ? monkey.TrueMonkey : monkey.FalseMonkey;
                    monkeys[targetMonkey].Items.Enqueue(worryLevel);
                }
            }
        }

        var orderedInspections = monkeyInspections.OrderByDescending(i => i).ToArray();
        
        return orderedInspections[0] * orderedInspections[1]; 
    }

    private (List<Monkey> Monkeys, int Modulo) ParseMonkeys()
    {
        int ParseMonkeyNumber(string input)
        {
            var number = input
                .Split(" ")[1]
                .TrimEnd(':');

            return int.Parse(number);
        }

        Queue<long> ParseItems(string input)
        {
            var items = input
                .Split(":")[1]
                .TrimStart()
                .Replace(",", string.Empty)
                .Split(" ")
                .Select(long.Parse);

            var itemQueue = new Queue<long>();

            foreach (var item in items)
            {
                itemQueue.Enqueue(item);
            }

            return itemQueue;
        }

        Func<long, long> ParseOperationExpression(string input)
        {
            var expression = input.Split(" ");

            if (long.TryParse(expression[^1], out var value))
            {
                return expression[^2] switch
                {
                    "+" => i => i + value,
                    "*" => i => i * value,
                    _ => throw new ArgumentException(input)
                };
            }

            return expression[^2] switch
            {
                "+" => i => i + i,
                "*" => i => i * i,
                _ => throw new ArgumentException(input)
            };
        }

        int ParseMonkeyTarget(string input)
        {
            return int.Parse(input.Split(" ").Last());
        }

        var monkeys = new List<Monkey>();

        for (var i = 0; i < _input.Length; i += 7)
        {
            var monkeyNumber = ParseMonkeyNumber(_input[i]);
            var items = ParseItems(_input[i + 1]);
            var operation = ParseOperationExpression(_input[i + 2]);
            var trueMonkey = ParseMonkeyTarget(_input[i + 4]);
            var falseMonkey = ParseMonkeyTarget(_input[i + 5]);
            var mod = int.Parse(_input[i + 3].Split(" ").Last());
            
            var monkey = new Monkey(monkeyNumber, items, operation, mod, trueMonkey, falseMonkey);

            monkeys.Add(monkey);
        }

        var modulo = monkeys.Aggregate(1, (mod, monkey) => mod * monkey.Modulo);

        Console.WriteLine(modulo);
        
        return (monkeys, modulo);
    }
    private record Monkey(int Number, Queue<long> Items, Func<long, long> Operation, int Modulo, int TrueMonkey, int FalseMonkey);
}
