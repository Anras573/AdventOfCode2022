namespace AdventOfCode;

public class Day21 : BaseDay
{
    private readonly string[] _input;

    public Day21()
    {
        _input = File.ReadAllLines(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part1()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part2()}");

    private long Part1()
    {
        var monkeys = new Dictionary<string, Monkey>();

        foreach (var line in _input)
        {
            var split = line.Split(" ");
            var name = line[..4];

            long Value()
            {
                if (char.IsDigit(split[1][0]))
                {
                    return long.Parse(split[1]);
                }

                return split[2] switch
                {
                    "+" => monkeys[split[1]].Value() + monkeys[split[3]].Value(),
                    "-" => monkeys[split[1]].Value() - monkeys[split[3]].Value(),
                    "/" => monkeys[split[1]].Value() / monkeys[split[3]].Value(),
                    "*" => monkeys[split[1]].Value() * monkeys[split[3]].Value(),
                    _ => throw new ArgumentException($"Unknown command: {split[2]} => found in line {line}")
                };
            }

            monkeys.Add(name, new Monkey(name, Value));
        }

        var sum = monkeys["root"].Value();
        
        return sum;
    }
    
    private long Part2()
    {
        var expression = Parse("root") as Equality;

        while (expression is { Left: not Human })
        {
            expression = SolveExpression(expression);
        }

        if (expression is { Right: Constant sum }) return sum.Value;

        throw new ArgumentException($"Expression is not a Constant! {Environment.NewLine}{expression?.Right}");
    }

    private static Equality SolveExpression(Equality equality)
    {
        return equality.Left switch
        {
            Operation(Constant left, "+", { } right)
                => new Equality(right, new Operation(equality.Right, "-", left).Calculate()),
            Operation(Constant left, "*", { } right)
                => new Equality(right, new Operation(equality.Right, "/", left).Calculate()),
            Operation({ } left, "+", { } right)
                => new Equality(left, new Operation(equality.Right, "-", right).Calculate()),
            Operation({ } left, "-", { } right)
                => new Equality(left, new Operation(equality.Right, "+", right).Calculate()),
            Operation({ } left, "*", { } right)
                => new Equality(left, new Operation(equality.Right, "/", right).Calculate()),
            Operation({ } left, "/", { } right)
                => new Equality(left, new Operation(equality.Right, "*", right).Calculate()),
            Constant => new Equality(equality.Right, equality.Left),
            _ => equality
        };
    }

    private IExpression Parse(string name)
    {
        var inputLookUp = new Dictionary<string, string[]>();
        
        foreach (var line in _input)
        {
            var split = line.Split(" ");
            inputLookUp[line[..4]] = split.Skip(1).ToArray();
        }

        IExpression Build(string lookUpName)
        {
            var split = inputLookUp[lookUpName];
            
            switch (lookUpName)
            {
                case "humn":
                    return new Human(lookUpName);
                case "root":
                    return new Equality(Build(split[0]), Build(split[2]));
            }

            if (split.Length == 1)
                return new Constant(long.Parse(split[0]));

            return new Operation(Build(split[0]), split[1], Build(split[2]));
        }

        return Build(name);
    }

    private record Monkey(string Name, Func<long> Value);

    private interface IExpression
    {
        IExpression Calculate();
    }

    private record Human(string Name) : IExpression
    {
        public IExpression Calculate() => this;
    }

    private record Constant(long Value) : IExpression
    {
        public IExpression Calculate() => this;
    }

    private record Equality(IExpression Left, IExpression Right) : IExpression
    {
        public IExpression Calculate() => new Equality(Left.Calculate(), Right.Calculate());
    }

    private record Operation(IExpression Left, string Operator, IExpression Right) : IExpression
    {
        public IExpression Calculate()
        {
            return (Left.Calculate(), Operator, Right.Calculate()) switch
            {
                (Constant left, "+", Constant right) => new Constant(left.Value + right.Value),
                (Constant left, "-", Constant right) => new Constant(left.Value - right.Value),
                (Constant left, "*", Constant right) => new Constant(left.Value * right.Value),
                (Constant left, "/", Constant right) => new Constant(left.Value / right.Value),
                ({ } left, _, { } right) => new Operation(left, Operator, right)
            };
        }
    }
}