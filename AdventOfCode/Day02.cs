namespace AdventOfCode;

public class Day02 : BaseDay
{
    Dictionary<string, HandShape> HandShapeDictionary = new()
    {
        { "A", HandShape.Rock },
        { "B", HandShape.Paper },
        { "C", HandShape.Scissor },
        { "X" , HandShape.Rock },
        { "Y", HandShape.Paper },
        { "Z", HandShape.Scissor }
    };
    
    Dictionary<string, GameState> StrategyDictionary = new()
    {
        { "X", GameState.Lost },
        { "Y", GameState.Draw },
        { "Z", GameState.Won }
    };
    
    private readonly string[] _input;

    public Day02()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {PointsByGuide()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {PointsByExplanation()}");

    private int PointsByGuide()
    {
        var sum = 0;

        foreach (var line in _input)
        {
            var actions = line.Split(" ");
            var (opponent, me) = (HandShapeDictionary[actions[0]], HandShapeDictionary[actions[1]]);

            sum += (int) me;
            sum += opponent switch
            {
                HandShape.Rock => me switch
                {
                    HandShape.Rock => (int)GameState.Draw,
                    HandShape.Paper => (int)GameState.Won,
                    _ => (int)GameState.Lost
                },
                HandShape.Paper => me switch
                {
                    HandShape.Rock => (int)GameState.Lost,
                    HandShape.Paper => (int)GameState.Draw,
                    _ => (int)GameState.Won
                },
                HandShape.Scissor => me switch
                {
                    HandShape.Rock => (int)GameState.Won,
                    HandShape.Paper => (int)GameState.Lost,
                    _ => (int)GameState.Draw
                },
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        return sum;
    }
    
    private int PointsByExplanation()
    {
        var sum = 0;

        foreach (var line in _input)
        {
            var actions = line.Split(" ");
            var opponent = HandShapeDictionary[actions[0]];
            var me = StrategyDictionary[actions[1]];

            sum += (int) me;
            sum += opponent switch
            {
                HandShape.Rock => me switch
                {
                    GameState.Draw => (int)HandShape.Rock,
                    GameState.Lost => (int)HandShape.Scissor,
                    _ => (int)HandShape.Paper
                },
                HandShape.Paper => me switch
                {
                    GameState.Draw => (int)HandShape.Paper,
                    GameState.Lost => (int)HandShape.Rock,
                    _ => (int)HandShape.Scissor
                },
                HandShape.Scissor => me switch
                {
                    GameState.Draw => (int)HandShape.Scissor,
                    GameState.Lost => (int)HandShape.Paper,
                    _ => (int)HandShape.Rock
                },
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        return sum;
    }
}

internal enum HandShape
{
    Rock = 1,
    Paper = 2,
    Scissor = 3
}

internal enum GameState
{
    Lost = 0,
    Draw = 3,
    Won = 6
}