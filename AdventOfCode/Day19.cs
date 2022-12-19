using System.Text.RegularExpressions;

namespace AdventOfCode;

public partial class Day19 : BaseDay
{
    private readonly string[] _input;

    [GeneratedRegex("\\D+")]
    private static partial Regex GetNumbers();
    
    public Day19()
    {
        _input = File.ReadAllLines(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part1()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part2()}");

    private int Part1()
    {
        var sum = 0;

        const int timeRemaining = 24;
        
        foreach (var blueprint in _input.Select(Blueprint.Parse))
        {
            var best = CalculateBestResult(blueprint, timeRemaining);

            sum += blueprint.Id * best;
        }
        
        return sum;
    }
    
    private int Part2()
    {
        var sum = 1;

        const int timeRemaining = 32;
        
        foreach (var blueprint in _input.Take(3).Select(Blueprint.Parse))
        {
            var best = CalculateBestResult(blueprint, timeRemaining);

            sum *= best;
        }
        
        return sum;
    }

    private static int CalculateBestResult(Blueprint blueprint, int timeRemaining)
    {
        var stateCache = new HashSet<State>();
        var queue = new Queue<State>();
        queue.Enqueue(new State(0, 0, 0, 0, 1, 0, 0, 0, timeRemaining));

        var best = 0;
        
        while (queue.TryDequeue(out var state))
        {
            best = int.Max(state.CrackedGeodeCount, best);

            if (state.TimeRemaining == 0) continue;

            var oreRobotCount = Math.Min(state.OreRobotCount, blueprint.HighestOreCost);
            var clayRobotCount = Math.Min(state.ClayRobotCount, blueprint.ObsidianRobotCost.Clay);
            var obsidianRobotCount = Math.Min(state.ObsidianRobotCount, blueprint.GeodeCrackerRobotCost.Obsidian);

            var timeRemainingOffset = state.TimeRemaining - 1;

            int CalculateProduction(int a, int b, int c, int d)
            {
                return a * b - c * d;
            }

            var calculatedOreProduction = CalculateProduction(
                state.TimeRemaining,
                blueprint.HighestOreCost,
                oreRobotCount,
                timeRemainingOffset);

            var calculatedClayProduction = CalculateProduction(
                state.TimeRemaining,
                blueprint.ObsidianRobotCost.Clay,
                clayRobotCount,
                timeRemainingOffset);

            var calculatedObsidianProduction = CalculateProduction(
                state.TimeRemaining,
                blueprint.GeodeCrackerRobotCost.Obsidian,
                obsidianRobotCount,
                timeRemainingOffset);

            var ore = Math.Min(state.Ore, calculatedOreProduction);
            var clay = Math.Min(state.Clay, calculatedClayProduction);
            var obsidian = Math.Min(state.Obsidian, calculatedObsidianProduction);

            var newState = new State(
                ore,
                clay,
                obsidian,
                state.CrackedGeodeCount,
                oreRobotCount,
                clayRobotCount,
                obsidianRobotCount,
                state.GeodeCrackerRobotCount,
                state.TimeRemaining);

            if (stateCache.Contains(newState)) continue;

            stateCache.Add(newState);

            var queueableState = newState with
            {
                Ore = ore + oreRobotCount,
                Clay = clay + clayRobotCount,
                Obsidian = obsidian + obsidianRobotCount,
                CrackedGeodeCount = newState.CrackedGeodeCount + newState.GeodeCrackerRobotCount,
                TimeRemaining = newState.TimeRemaining - 1
            };

            queue.Enqueue(queueableState);

            if (ore >= blueprint.OreRobotCost)
                queue.Enqueue(queueableState with
                {
                    Ore = queueableState.Ore - blueprint.OreRobotCost,
                    OreRobotCount = queueableState.OreRobotCount + 1
                });

            if (ore >= blueprint.ClayRobotCost)
                queue.Enqueue(queueableState with
                {
                    Ore = queueableState.Ore - blueprint.ClayRobotCost,
                    ClayRobotCount = queueableState.ClayRobotCount + 1
                });

            if (ore >= blueprint.ObsidianRobotCost.Ore && clay >= blueprint.ObsidianRobotCost.Clay)
                queue.Enqueue(queueableState with
                {
                    Ore = queueableState.Ore - blueprint.ObsidianRobotCost.Ore,
                    Clay = queueableState.Clay - blueprint.ObsidianRobotCost.Clay,
                    ObsidianRobotCount = queueableState.ObsidianRobotCount + 1
                });

            if (ore >= blueprint.GeodeCrackerRobotCost.Ore && obsidian >= blueprint.GeodeCrackerRobotCost.Obsidian)
                queue.Enqueue(queueableState with
                {
                    Ore = queueableState.Ore - blueprint.GeodeCrackerRobotCost.Ore,
                    Obsidian = queueableState.Obsidian - blueprint.GeodeCrackerRobotCost.Obsidian,
                    GeodeCrackerRobotCount = queueableState.GeodeCrackerRobotCount + 1
                });
        }

        return best;
    }

    private record struct State(
        int Ore,
        int Clay,
        int Obsidian,
        int CrackedGeodeCount,
        int OreRobotCount,
        int ClayRobotCount,
        int ObsidianRobotCount,
        int GeodeCrackerRobotCount,
        int TimeRemaining);
    
    private record struct Blueprint(
        int Id,
        int OreRobotCost,
        int ClayRobotCost,
        (int Ore, int Clay) ObsidianRobotCost,
        (int Ore, int Obsidian) GeodeCrackerRobotCost,
        int HighestOreCost)
    {
        public static Blueprint Parse(string input)
        {
            var values = GetNumbers()
                .Split(input)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(int.Parse)
                .ToList();
            
            var id = values[0];
            var oreRobotCost = values[1];
            var clayRobotCost = values[2];
            var obsidianRobotCost = (Ore: values[3], Clay: values[4]);
            var geodeCrackerRobotCost = (Ore: values[5], Obsidian: values[6]);

            var oreCosts = new List<int>
            {
                oreRobotCost, clayRobotCost, obsidianRobotCost.Ore, geodeCrackerRobotCost.Ore
            };
            
            var highestOreCost = oreCosts.Max();

            return new Blueprint(id, oreRobotCost, clayRobotCost, obsidianRobotCost, geodeCrackerRobotCost, highestOreCost);
        }
    };
}