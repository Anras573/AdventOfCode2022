namespace AdventOfCode;

public class Day15 : BaseDay
{
    private readonly string[] _input;

    public Day15()
    {
        _input = File.ReadAllLines(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part1(2_000_000)}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part2(4_000_000)}");

    private int Part1(int target)
    {
        var sensors = _input.Select(Sensor.Parse).ToList();
        var beacons = sensors.Select(s => s.Beacon).ToHashSet();
        
        var notBeacons = new HashSet<Coordinate>();

        foreach (var sensor in sensors)
        {
            var radius = GetDistance(sensor.Coord, sensor.Beacon);
            var distance = Math.Abs(sensor.Coord.Y - target);

            if (distance > radius)
                continue;
            
            var offset = radius - distance;
            
            for (var x = sensor.Coord.X - offset; x <= sensor.Coord.X + offset; x++)
            {
                var newCoord = new Coordinate(x, target);
                
                if (!beacons.Contains(newCoord))
                    notBeacons.Add(newCoord);
            }
        }

        return notBeacons.Count;
    }
    
    private long Part2(long target)
    {
        var sensors = _input.Select(Sensor.Parse).ToList();

        var directions = new[]
        {
            new Coordinate(-1, -1),
            new Coordinate(-1, 1),
            new Coordinate(1, -1),
            new Coordinate(1, 1),
        };

        bool IsTargetBeacon(int x, int y)
        {
            foreach (var sensor in sensors)
            {
                var dist1 = GetDistance(sensor.Coord, sensor.Beacon);
                var dist2 = GetDistance(sensor.Coord, new Coordinate(x, y));

                if (dist2 < dist1)
                    return false;
            }

            return true;
        }

        foreach (var sensor in sensors)
        {
            var distance = GetDistance(sensor.Coord, sensor.Beacon);
            
            foreach (var direction in directions)
            {
                for (var dx = 0; dx <= distance + 1; dx++)
                {
                    var dy = distance + 1 - dx;
                    var x = sensor.Coord.X + dx * direction.X;
                    var y = sensor.Coord.Y + dy * direction.Y;

                    if (x < 0 || y < 0 || x > target || y > target)
                        continue;

                    if (IsTargetBeacon(x, y))
                        return x * target + y;
                }
            }
        }
        
        return 0;
    }
    
    private static int GetDistance(Coordinate first, Coordinate second)
        => Math.Abs(first.X - second.X) + Math.Abs(first.Y - second.Y);

    private record struct Coordinate(int X, int Y)
    {
        public static Coordinate Parse(string input)
        {
            var parsedInput = input
                .Replace("x=", string.Empty)
                .Replace(" y=", string.Empty)
                .Split(',')
                .Select(int.Parse)
                .ToList();

            return new Coordinate(parsedInput[0], parsedInput[1]);
        }
    };

    private record Sensor(Coordinate Coord, Coordinate Beacon)
    {
        public static Sensor Parse(string input)
        {
            var data = input.Split(':');
            var sensorCoords = data[0]
                .Replace("Sensor at ", string.Empty);

            var sensor = Coordinate.Parse(sensorCoords);

            var beaconCoords = data[1]
                .Replace(" closest beacon is at ", string.Empty);

            var beacon = Coordinate.Parse(beaconCoords);
            
            return new Sensor(sensor, beacon);
        }
    };
}

