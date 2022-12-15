namespace AdventOfCode;

public class Day12 : BaseDay
{
    private readonly string[] _input;

    public Day12()
    {
        _input = File.ReadAllLines(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part1()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part2()}");

    private int Part1()
    {
        return ShortestPath('S');
    }
    
    private int Part2()
    {
        return ShortestPath('a');
    }

    private int ShortestPath(char target)
    {
        bool IsWithinRange(Node node, char c)
        {
            var value = ConvertFromCharToInt(c);

            return node.Value - value <= 1;
        }
        
        var grid = ParseGrid();
        
        var root = new Node(new Point(0, 0), 0, 26);
        
        var visited = new bool[grid.Length, grid[0].Length];
        var found = false;
        
        for (var row = 0; row < grid.Length; row++) {
            for (var column = 0; column < grid[row].Length; column++) {
                if (grid[row][column] == 'E')
                {
                    root = root with { Point = new Point(row, column) };
                    found = true;
                    break;
                }
            }

            if (found)
                break;
        }
        
        var queue = new Queue<Node>();
        queue.Enqueue(root);
        visited[root.Point.X, root.Point.Y] = true;
        
        while (queue.Count > 0) {
            var node = queue.Dequeue();
            var p = node.Point;
 
            if (grid[p.X][p.Y] == target) {
                return node.Distance;
            }
            
            if (p.X - 1 >= 0 && !visited[p.X - 1, p.Y] && IsWithinRange(node, grid[p.X - 1][p.Y])) {
                queue.Enqueue(new Node(p with { X = p.X - 1 }, node.Distance + 1, ConvertFromCharToInt(grid[p.X - 1][p.Y])));
                visited[p.X - 1, p.Y] = true;
            }
            
            if (p.X + 1 < grid.Length && !visited[p.X + 1, p.Y] && IsWithinRange(node, grid[p.X + 1][p.Y])) {
                queue.Enqueue(new Node(p with { X = p.X + 1 }, node.Distance + 1, ConvertFromCharToInt(grid[p.X + 1][p.Y])));
                visited[p.X + 1, p.Y] = true;
            }
            
            if (p.Y - 1 >= 0 && !visited[p.X, p.Y - 1] && IsWithinRange(node, grid[p.X][p.Y - 1])) {
                queue.Enqueue(new Node(p with { Y = p.Y - 1 }, node.Distance + 1, ConvertFromCharToInt(grid[p.X][p.Y - 1])));
                visited[p.X, p.Y - 1] = true;
            }
            
            if (p.Y + 1 < grid[0].Length && !visited[p.X, p.Y + 1] && IsWithinRange(node, grid[p.X][p.Y + 1])) {
                queue.Enqueue(new Node(p with { Y = p.Y + 1 }, node.Distance + 1, ConvertFromCharToInt(grid[p.X][p.Y + 1])));
                visited[p.X, p.Y + 1] = true;
            }
        }
        
        return -1;
    }
    
    private char[][] ParseGrid()
    {
        var grid = new char[_input.Length][];

        for (var i = 0; i < _input.Length; i++)
        {
            grid[i] = _input[i].ToCharArray();
        }

        return grid;
    }
    
    private static int ConvertFromCharToInt(char c)
    {
        return c switch
        {
            'S' => 1,
            'E' => 26,
            _ => c - 96
        };
    }
    private record Node(Point Point, int Distance, int Value);
    private record Point(int X, int Y);
}
