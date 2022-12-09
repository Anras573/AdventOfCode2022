namespace AdventOfCode;

public class Day09 : BaseDay
{
    private readonly string[] _input;
    private readonly Dictionary<string, Position> _movements = new()
    {
        { "R", new Position(1, 0) },
        { "U", new Position(0, 1) },
        { "L", new Position(-1, 0) },
        { "D", new Position(0, -1) },
    };

    public Day09()
    {
        _input = File.ReadAllLines(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part1()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part2()}");

    private int Part1()
    {
        var head = new Position(0, 0);
        var tail = new Position(0, 0);
        var visitedPositions = new HashSet<Position> { tail };

        foreach (var line in _input)
        {
            var command = line.Split(" ");
            var movement = _movements[command[0]];

            var count = int.Parse(command[1]);
            while (count != 0)
            {
                head.X += movement.X;
                head.Y += movement.Y;
                
                if (tail.X < head.X - 1)
                {
                    tail.X += 1;
                    
                    if (tail.Y < head.Y)
                    {
                        tail.Y += 1;
                    }
                    if (tail.Y > head.Y)
                    {
                        tail.Y += -1;
                    }
                }

                if (tail.Y < head.Y - 1)
                {
                    tail.Y += 1;
                    
                    if (tail.X < head.X)
                    {
                        tail.X += 1;
                    }
                    if (tail.X > head.X)
                    {
                        tail.X += -1;
                    }
                }

                if (tail.X > head.X + 1)
                {
                    tail.X += -1;
                    
                    if (tail.Y < head.Y)
                    {
                        tail.Y += 1;
                    }
                    if (tail.Y > head.Y)
                    {
                        tail.Y += -1;
                    }
                }

                if (tail.Y > head.Y + 1)
                {
                    tail.Y += -1;
                    
                    if (tail.X < head.X)
                    {
                        tail.X += 1;
                    }
                    if (tail.X > head.X)
                    {
                        tail.X += -1;
                    }
                }

                visitedPositions.Add(tail);

                count--;
            }
        }
        
        return visitedPositions.Count;
    }
    
    private int Part2()
    {
        var rope = new Position[]
        {
            new Position(0, 0),
            new Position(0, 0),
            new Position(0, 0),
            new Position(0, 0),
            new Position(0, 0),
            new Position(0, 0),
            new Position(0, 0),
            new Position(0, 0),
            new Position(0, 0),
            new Position(0, 0)
        };
        
        var visitedPositions = new HashSet<Position> { rope[9] };

        foreach (var line in _input)
        {
            var command = line.Split(" ");
            var movement = _movements[command[0]];

            var count = int.Parse(command[1]);
            var head = rope[0];
            while (count != 0)
            {
                head.X += movement.X;
                head.Y += movement.Y;

                rope[0] = head;

                for (var i = 1; i < rope.Length; i++)
                {
                    var lead = rope[i - 1];
                    var tail = rope[i];
                    
                    if (tail.X < lead.X - 1)
                    {
                        tail.X += 1;
                    
                        if (tail.Y < lead.Y)
                        {
                            tail.Y += 1;
                        }
                        if (tail.Y > lead.Y)
                        {
                            tail.Y += -1;
                        }
                    }

                    if (tail.Y < lead.Y - 1)
                    {
                        tail.Y += 1;
                    
                        if (tail.X < lead.X)
                        {
                            tail.X += 1;
                        }
                        if (tail.X > lead.X)
                        {
                            tail.X += -1;
                        }
                    }

                    if (tail.X > lead.X + 1)
                    {
                        tail.X += -1;
                    
                        if (tail.Y < lead.Y)
                        {
                            tail.Y += 1;
                        }
                        if (tail.Y > lead.Y)
                        {
                            tail.Y += -1;
                        }
                    }

                    if (tail.Y > lead.Y + 1)
                    {
                        tail.Y += -1;
                    
                        if (tail.X < lead.X)
                        {
                            tail.X += 1;
                        }
                        if (tail.X > lead.X)
                        {
                            tail.X += -1;
                        }
                    }

                    rope[i - 1] = lead;
                    rope[i] = tail;
                }
                
                visitedPositions.Add(rope[9]);

                count--;
            }
        }

        return visitedPositions.Count;
    }
}

public record struct Position(int X, int Y);