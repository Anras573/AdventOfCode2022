using System.Text;

namespace AdventOfCode;

public class Day22 : BaseDay
{
    private readonly string[] _input;

    private static readonly Dictionary<Direction, int> DirectionValueLookUp = new ()
    {
        { Direction.E, 0 },
        { Direction.S, 1 },
        { Direction.W, 2 },
        { Direction.N, 3 }
    };
    
    private static readonly Dictionary<Vector2, int> VectorDirectionValueLookUp = new ()
    {
        { new Vector2(1, 0), 0 },
        { new Vector2(0, 1), 1 },
        { new Vector2(-1, 0), 2 },
        { new Vector2(0, -1), 3 }
    };

    private readonly Dictionary<Vector2, char> _dictionaryMap;
    private readonly char[,] _map2D;
    private readonly string[] _instructions;
    private readonly Size _size;

    public Day22()
    {
        _input = File.ReadAllLines(InputFilePath);
        (_dictionaryMap, _map2D, _instructions, _size) = ParseInput();
    }
    
    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part1()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part2()}");

    private int Part1()
    {
        var currentLocation = _dictionaryMap.Keys.Where(c => c.Y == 1).MinBy(c => c.X);
        var currentDirection = Direction.E;

        foreach (var instruction in _instructions)
        {
            if (char.IsLetter(instruction[0]))
            {
                currentDirection = instruction == "L" ? TurnLeft(currentDirection) : TurnRight(currentDirection);
                continue;
            }

            var steps = int.Parse(instruction);
            for (var i = 0; i < steps; i++)
            {
                if (_dictionaryMap.TryGetValue(currentLocation.Move(currentDirection), out var nextValue))
                {
                    if (nextValue == '.')
                        currentLocation = currentLocation.Move(currentDirection);
                    else
                        break;
                }
                else
                {
                    var tryDirection = TurnLeft(TurnLeft(currentDirection));
                    var testLocation = currentLocation.Move(tryDirection, 50);
                    
                    while (_dictionaryMap.ContainsKey(testLocation))
                        testLocation = testLocation.Move(tryDirection, 50);

                    testLocation = testLocation.Move(currentDirection);

                    var loopedAround = _dictionaryMap[testLocation];
                    if (loopedAround == '.')
                        currentLocation = testLocation;
                    else
                        break;
                }
            }
        }

        var directionValue = DirectionValueLookUp[currentDirection];
        
        return 1000 * currentLocation.Y + 4 * currentLocation.X + directionValue;
    }

    private int Part2()
    {
        var cubeSize = CalculateCubeSize();

        var cubeFaces = GenerateCubeFaces(cubeSize);

        var currentFace = GenerateLayout(cubeFaces, cubeSize);

        var currentPosition = new Vector3(0, 0, 0);

        var currentFacing = currentFace.XDirection;

        foreach (var instruction in _instructions)
        {
            if (char.IsLetter(instruction[0]))
            {
                currentFacing = instruction == "L"
                    ? Vector3.RotateLeft(currentFacing, currentFace.Normal)
                    : Vector3.RotateRight(currentFacing, currentFace.Normal);
                continue;
            }
            
            var steps = int.Parse(instruction);

            for (var i = 0; i < steps; i++)
            {
                var position = currentPosition;
                var face = currentFace;
                var facing = currentFacing;

                currentPosition += currentFacing;

                var projection = currentFace.Projection(currentPosition);

                if (projection.X < 0)
                {
                    currentPosition = position;
                    currentFacing = -face.Normal;
                    currentFace = cubeFaces.Values.First(f => f.Normal == -currentFace.XDirection);
                }
                else if (projection.Y < 0)
                {
                    currentPosition = position;
                    currentFacing = -face.Normal;
                    currentFace = cubeFaces.Values.First(f => f.Normal == -currentFace.YDirection);
                }
                else if (projection.X >= cubeSize)
                {
                    currentPosition = position;
                    currentFacing = -face.Normal;
                    currentFace = cubeFaces.Values.First(f => f.Normal == currentFace.XDirection);
                }
                else if (projection.Y >= cubeSize)
                {
                    currentPosition = position;
                    currentFacing = -face.Normal;
                    currentFace = cubeFaces.Values.First(f => f.Normal == currentFace.YDirection);
                }

                projection = currentFace.Projection(currentPosition);

                if (currentFace.Map[projection.X, projection.Y] == '.') continue;
                
                currentPosition = position;
                currentFacing = facing;
                currentFace = face;
            }
        }

        var lastProjection = currentFace.Projection(currentPosition);
        var direction = currentFace.Projection(currentPosition + currentFacing) - lastProjection;

        lastProjection += currentFace.Origin;

        return 1000 * (lastProjection.Y + 1) + 4 * (lastProjection.X + 1) + VectorDirectionValueLookUp[direction];
    }

    private CubeFace GenerateLayout(Dictionary<Vector2, CubeFace> cubeFaces, int cubeSize)
    {
        var first = cubeFaces.First().Value with
        {
            Normal = new Vector3(0, 0, -1),
            XDirection = new Vector3(1, 0, 0),
            YDirection = new Vector3(0, 1, 0)
        };

        cubeFaces[first.Origin] = first;

        var queue = new Queue<CubeFace>();
        queue.Enqueue(first);

        while (queue.Count > 0)
        {
            var face = queue.Dequeue();
            
            var checkFace = face.Origin with { X = face.Origin.X + cubeSize };

            if (cubeFaces.TryGetValue(checkFace, out var right) && right.Normal.Length == 0)
            {
                var newRightFace = right with
                {
                    Normal = face.XDirection, YDirection = face.YDirection, XDirection = -face.Normal
                };

                queue.Enqueue(newRightFace);
                cubeFaces[checkFace] = newRightFace;
            }

            checkFace = face.Origin with { X = face.Origin.X - cubeSize };

            if (cubeFaces.TryGetValue(checkFace, out var left) && left.Normal.Length == 0)
            {
                var newLeftFace = left with
                {
                    Normal = -face.XDirection, YDirection = face.YDirection, XDirection = face.Normal
                };

                queue.Enqueue(newLeftFace);
                cubeFaces[checkFace] = newLeftFace;
            }


            checkFace = face.Origin with { Y = face.Origin.Y + cubeSize };

            if (cubeFaces.TryGetValue(checkFace, out var down) && down.Normal.Length == 0)
            {
                var newDownFace = down with
                {
                    Normal = face.YDirection, YDirection = -face.Normal, XDirection = face.XDirection
                };

                queue.Enqueue(newDownFace);
                cubeFaces[checkFace] = newDownFace;
            }

            checkFace = face.Origin with { Y = face.Origin.Y - cubeSize };

            if (cubeFaces.TryGetValue(checkFace, out var up) && up.Normal.Length == 0)
            {
                var newUpFace = up with
                {
                    Normal = -face.YDirection, YDirection = face.Normal, XDirection = face.XDirection
                };

                queue.Enqueue(newUpFace);
                cubeFaces[checkFace] = newUpFace;
            }
        }

        return GetStartingFace(cubeFaces, cubeSize);
    }
    
    private CubeFace GetStartingFace(Dictionary<Vector2, CubeFace> cubeFaces, int cubeSize)
    {
        var x = 0;
        
        while (_map2D[x, 0] != '.')
            x++;

        var p = new Vector2(x / cubeSize * cubeSize, 0);
        
        return cubeFaces[p];
    }

    private Dictionary<Vector2, CubeFace> GenerateCubeFaces(int cubeSize)
    {
        var cubeFaces = new Dictionary<Vector2, CubeFace>();

        for (var y = 0; y < _size.Height; y += cubeSize)
        {
            for (var x = 0; x < _size.Width; x += cubeSize)
            {
                if (_map2D[x, y] == ' ')
                {
                    continue;
                }

                var map = new char[cubeSize, cubeSize];
                var origin = new Vector2(x, y);
                var normal = new Vector3(0, 0, 0);
                var xDirection = new Vector3(0, 0, 0);
                var yDirection = new Vector3(0, 0, 0);
                var face = new CubeFace(map, origin, normal, xDirection, yDirection);

                for (var i = 0; i < cubeSize; i++)
                {
                    for (var j = 0; j < cubeSize; j++)
                    {
                        face.Map[i, j] = _map2D[i + x, j + y];
                    }
                }

                cubeFaces.Add(origin, face);
            }
        }

        return cubeFaces;
    }

    private int CalculateCubeSize()
    {
        var cubeSize = int.MaxValue;

        for (var x = 0; x < _size.Width; x++)
        {
            var counter = 0;
            for (var y = 0; y < _size.Height; y++)
            {
                if (_map2D[x, y] == ' ')
                {
                    if (counter > 0)
                    {
                        cubeSize = Math.Min(cubeSize, counter);
                    }

                    counter = 0;
                }
                else
                {
                    counter++;
                }
            }
        }

        return cubeSize;
    }

    private (Dictionary<Vector2, char> map, char[,] map2D, string[] instructions, Size size) ParseInput()
    {
        var map = new Dictionary<Vector2, char>();
        var instructions = Array.Empty<string>();

        var isNextPart = false;
        var y = 1;

        var size = new Size(0, 0);
        
        foreach (var line in _input)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                isNextPart = true;
                continue;
            }

            if (isNextPart)
            {
                instructions = TokenizeInput(line);
                continue;
            }
            
            for(var x = 0; x < line.Length; x++)
            {
                if (char.IsWhiteSpace(line[x])) continue;
                
                map[new Vector2(x + 1, y)] = line[x];
            }
            y++;

            size.Width = Math.Max(line.Length, size.Width);
            size.Height++;
        }

        var map2D = new char[size.Width, size.Height];
        y = 0;
        
        foreach (var line in _input)
        {
            if (line.Length == 0)
                break;
             
            for (var x = 0; x < line.Length; x++)
                map2D[x, y] = line[x];

            for (var x = line.Length; x < size.Width; x++)
                map2D[x, y] = ' ';

            y++;
        }
        
        return (map, map2D, instructions, size);
    }
    
    private static string[] TokenizeInput(string line)
    {
        var words = new List<StringBuilder> { new() };
        for (var i = 0; i < line.Length; i++)
        {
            words[^1].Append(line[i]);
            if (i + 1 < line.Length && char.IsLetter(line[i]) != char.IsLetter(line[i + 1]))
            {
                words.Add(new StringBuilder());
            }
        }

        return words.Select(x => x.ToString()).ToArray();
    }
    
    private readonly record struct Vector2(int X, int Y)
    {
        public Vector2 Move(Direction direction, int movement = 1)
        {
            return direction switch
            {
                Direction.N => this with { Y = Y - movement },
                Direction.E => this with { X = X + movement },
                Direction.S => this with { Y = Y + movement },
                Direction.W => this with { X = X - movement },
                _ => throw new ArgumentException($"Unknown Direction: {direction}", nameof(direction))
            };
        }

        public static Vector2 operator -(Vector2 left, Vector2 right)
            => new()
            {
                X = left.X - right.X,
                Y = left.Y - right.Y
            };
        
        public static Vector2 operator +(Vector2 left, Vector2 right)
            => new()
            {
                X = left.X + right.X,
                Y = left.Y + right.Y
            };
    }

    private record struct Size(int Width, int Height);

    private readonly record struct Vector3(int X, int Y, int Z)
    {
        public int Length => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
        public int MinimumComponent => Math.Min(X, Math.Min(Y, Z));
        
        public static Vector3 operator-(Vector3 a)
            => new (-a.X, -a.Y, -a.Z);
        
        public static Vector3 operator+(Vector3 left, Vector3 right)
            => new ()
            {
                X = left.X + right.X,
                Y = left.Y + right.Y,
                Z = left.Z + right.Z
            };

        public static int Dot(Vector3 left, Vector3 right)
            => left.X * right.X + left.Y * right.Y + left.Z * right.Z;

        private static Vector3 Cross(Vector3 left, Vector3 right)
            => new(
                X: left.Y * right.Z - left.Z * right.Y,
                Y: -(left.X * right.Z - left.Z * right.X),
                Z: left.X * right.Y - left.Y * right.X);

        public static Vector3 RotateRight(Vector3 facing, Vector3 axis) => Cross(facing, axis);
        public static Vector3 RotateLeft(Vector3 facing, Vector3 axis) => Cross(axis, facing);
    }

    private enum Direction { N = 0, E = 90, S = 180, W = 270 }

    private static Direction TurnLeft(Direction direction)
    {
        return direction switch
        {
            Direction.E => Direction.N,
            Direction.N => Direction.W,
            Direction.W => Direction.S,
            Direction.S => Direction.E,
            _ => throw new ArgumentException($"Invalid direction: {direction}", nameof(direction))
        };
    }
    
    private static Direction TurnRight(Direction direction)
    {
        return direction switch
        {
            Direction.E => Direction.S,
            Direction.S => Direction.W,
            Direction.W => Direction.N,
            Direction.N => Direction.E,
            _ => throw new ArgumentException($"Invalid direction: {direction}", nameof(direction))
        };
    }

    private record CubeFace(char[,] Map, Vector2 Origin, Vector3 Normal, Vector3 XDirection, Vector3 YDirection)
    {
        public Vector2 Projection(Vector3 position)
        {
            var x = Vector3.Dot(position, XDirection);
            var y = Vector3.Dot(position, YDirection);
            
            if (XDirection.MinimumComponent < 0)
            {
                x = Map.GetLength(0) - 1 + x;
            }

            if (YDirection.MinimumComponent < 0)
            {
                y = Map.GetLength(1) - 1 + y;
            }

            return new Vector2(x, y);
        }
    };
}