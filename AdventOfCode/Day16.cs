namespace AdventOfCode;

public class Day16 : BaseDay
{
    private readonly string[] _input;
    private readonly Dictionary<string, Valve> _valvesByName;
    private readonly Dictionary<int, Valve> _valvesById;
    private readonly Dictionary<KeyPair, int> _distances;
    private readonly bool[] _operableValves;

    public Day16()
    {
        _input = File.ReadAllLines(InputFilePath);

        var valves = _input
            .Select(Valve.Parse)
            .OrderByDescending(v => v.Flow)
            .Select((v, i) => v with { Id = i })
            .ToList();
        
        _valvesByName = valves.ToDictionary(v => v.Name, v => v);
        _valvesById = valves.ToDictionary(v => v.Id, v => v);
        _distances = CalculateDistances(valves, _valvesByName);
        _operableValves = valves.Select(v => v.Flow > 0).ToArray();
    }
    
    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part1()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part2()}");

    private int Part1()
    {
        var start = _valvesByName["AA"];
        var instructions = new List<Instruction>
        {
            new (start, 0),
            new (start, int.MaxValue)
        };
        
        return CalculateMaxFlow(instructions, _operableValves, 30);
    }
    
    private int Part2()
    {
        var start = _valvesByName["AA"];
        var instructions = new List<Instruction>
        {
            new (start, 0),
            new (start, 0)
        };
        
        return CalculateMaxFlow(instructions, _operableValves, 26);
    }

    private static Dictionary<KeyPair, int> CalculateDistances(List<Valve> valves, Dictionary<string, Valve> valvesByName)
    {
        var distances = new Dictionary<KeyPair, int>();

        for (var i = 0; i < valves.Count; i++)
        {
            for (var j = 0; j < valves.Count; j++)
            {
                distances[new KeyPair(i, j)] = int.MaxValue;
            }
        }

        foreach (var valve in valves)
        {
            foreach (var target in valve.Tunnels)
            {
                var targetNode = valvesByName[target];
                distances[new KeyPair(valve.Id, targetNode.Id)] = 1;
                distances[new KeyPair(targetNode.Id, valve.Id)] = 1;
            }
        }

        var go = true;

        while (go)
        {
            go = false;

            foreach (var first in valves)
            {
                foreach (var second in valves)
                {
                    if (first.Id == second.Id) continue;

                    foreach (var third in valves)
                    {
                        var startPath = new KeyPair(first.Id, third.Id);
                        var endPath = new KeyPair(third.Id, second.Id);

                        if (distances[startPath] == int.MaxValue ||
                            distances[endPath] == int.MaxValue)
                            continue;

                        var cost = distances[startPath] + distances[endPath];

                        var fsKey = new KeyPair(first.Id, second.Id);

                        if (cost >= distances[fsKey]) continue;

                        var sfKey = new KeyPair(second.Id, first.Id);

                        go = true;

                        distances[fsKey] = cost;
                        distances[sfKey] = cost;
                    }
                }
            }
        }

        return distances;
    }

    private int CalculateMaxFlow(
        List<Instruction> instructions,
        bool[] operableValves,
        int time,
        int maxFlow = 0,
        int currentFlow = 0)
    {
        var instructionsPerHelper = new List<Instruction>[instructions.Count];

        var newOperableValves = new bool[operableValves.Length];
        Array.Copy(operableValves, newOperableValves, operableValves.Length);

        for (var i = 0; i < instructions.Count; i++)
        {
            var instruction = instructions[i];
            
            if (instruction.DistanceToTarget > 0)
            {
                var ins = instruction with { DistanceToTarget = instruction.DistanceToTarget - 1 };
                instructionsPerHelper[i] = new List<Instruction> { ins };
            }
            else if (newOperableValves[instruction.Target.Id])
            {
                currentFlow += instruction.Target.Flow * (time - 1);

                if (currentFlow > maxFlow)
                {
                    maxFlow = currentFlow;
                }

                newOperableValves[instruction.Target.Id] = false;
            
                instructionsPerHelper[i] = new List<Instruction> { instruction };
            }
            else
            {
                instructionsPerHelper[i] = newOperableValves
                    .Select((value, index) => (value, index))
                    .Where(p => p.value)
                    .Select(p =>
                    {
                        var newTarget = _valvesById[p.index];
                        var distance = _distances[new KeyPair(instruction.Target.Id, newTarget.Id)];
            
                        return new Instruction(newTarget, distance - 1);
                    })
                    .ToList();
            }
        }

        time--;
        
        if (time < 1)
            return maxFlow;

        if (currentFlow + Residue(newOperableValves, time) <= maxFlow)
            return maxFlow;

        for (var i = 0; i < instructionsPerHelper[0].Count; i++)
        {
            for (var j = 0; j < instructionsPerHelper[1].Count; j++)
            {
                var instructions0 = instructionsPerHelper[0][i];
                var instructions1 = instructionsPerHelper[1][j];

                if ((instructionsPerHelper[0].Count > 1 || instructionsPerHelper[1].Count > 1)
                    && instructions0.Target.Id == instructions1.Target.Id)
                    continue;
                
                var advance = 0;
                
                if (instructions0.DistanceToTarget > 0 && instructions1.DistanceToTarget > 0)
                {
                    advance = Math.Min(instructions0.DistanceToTarget, instructions1.DistanceToTarget);
                    instructions0 = instructions0 with { DistanceToTarget = instructions0.DistanceToTarget - advance };
                    instructions1 = instructions1 with { DistanceToTarget = instructions1.DistanceToTarget - advance };
                }

                maxFlow = CalculateMaxFlow(
                    new List<Instruction> { instructions0, instructions1 },
                    newOperableValves,
                    time - advance,
                    maxFlow,
                    currentFlow);
            }
        }

        return maxFlow;
    }

    private int Residue(bool[] operableValves, int time)
    {
        var flow = 0;

        for (var i = 0; i < operableValves.Length; i++)
        {
            if (!operableValves[i]) continue;

            flow += _valvesById[i].Flow * (time - 1);

            if ((i & 1) == 0)
            {
                time--;
            }

            if (time <= 0) break;
        }
        
        return flow;
    }

    private record Valve(int Id, string Name, int Flow, List<string> Tunnels)
    {
        // Valve AA has flow rate=0; tunnels lead to valves DD, II, BB
        public static Valve Parse(string input)
        {
            var split = input.Split(" ");
            var name = split[1];
            var flow = int.Parse(split[4].TrimEnd(';').Replace("rate=", string.Empty));
            var tunnels = split.Skip(9).Select(v => v.TrimEnd(',')).ToList();

            return new Valve(0, name, flow, tunnels);
        }
    };
    private record Instruction(Valve Target, int DistanceToTarget);
    private record struct KeyPair(int X, int Y);
}