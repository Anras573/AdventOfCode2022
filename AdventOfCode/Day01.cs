namespace AdventOfCode;

public class Day01 : BaseDay
{
    private readonly string[] _input;

    public Day01()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {MaxCalories()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {MaxThreeCalories()}");

    private int MaxCalories()
    {
        var maxCalories = 0;
        var tempCalorie = 0;
        
        foreach (var calorie in _input)
        {
            if (string.IsNullOrWhiteSpace(calorie))
            {
                if (tempCalorie > maxCalories)
                {
                    maxCalories = tempCalorie;
                }
                tempCalorie = 0;
                continue;
            }
            
            tempCalorie += int.Parse(calorie);
        }
        
        return maxCalories;
    }
    
    private int MaxThreeCalories()
    {
        var max1Calories = 0;
        var max2Calories = 0;
        var max3Calories = 0;
        var tempCalorie = 0;
        
        foreach (var calorie in _input)
        {
            if (string.IsNullOrWhiteSpace(calorie))
            {
                if (tempCalorie > max1Calories)
                {
                    (max1Calories, max2Calories, max3Calories) = (tempCalorie, max1Calories, max2Calories);
                }
                else if (tempCalorie > max2Calories)
                {
                    (max2Calories, max3Calories) = (tempCalorie, max2Calories);
                }
                else if (tempCalorie > max3Calories)
                {
                    max3Calories = tempCalorie;
                }
                
                tempCalorie = 0;
                continue;
            }
            
            tempCalorie += int.Parse(calorie);
        }
        
        return max1Calories + max2Calories + max3Calories;
    }
}
