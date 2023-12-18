var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

long part1Result = 0;
long part2Result = 0;

var matrix = Array.Empty<string[]>();
await foreach (var line in File.ReadLinesAsync(filePath))
{
    matrix = matrix.Append(line.ToCharArray().Select(e => e.ToString()).ToArray()).ToArray();
}

(int y, int x) startingPosition = (0, 0);
for (var y = 0; y < matrix.Length; y++)
{
    var row = matrix[y];
    for (var x = 0; x < row.Length; x++)
    {
        if (row[x] is not StartingPosition)
        {
            if (GetConnectingCells(matrix, y, x).Count == 0)
            {
                matrix[y][x] = Ground;
            }
            continue;
        }

        startingPosition = (y, x);
    }
}

var result = FindPath(matrix, startingPosition.y, startingPosition.x, new List<(int y, int x, string part)>()
{
    (startingPosition.y, startingPosition.x, StartingPosition)
});

PrintMatrixFromPath(matrix, result);

part1Result = (result.Count - 1) / 2;

Console.WriteLine($"Part 1 result: {part1Result}");

internal partial class Program
{
    public const string TopLeftCorner = "F";
    public const string TopRightCorner = "7";
    public const string BottomLeftCorner = "L";
    public const string BottomRightCorner = "J";
    public const string Horizontal = "-";
    public const string Vertical = "|";
    public const string StartingPosition = "S";
    public const string Ground = ".";

    static List<(int y, int x, string part)> FindPath(string[][] matrix, int y, int x, List<(int y, int x, string part)> path)
    {
        if (path.Count > 3 && path.First().part is StartingPosition && path.Last().part is StartingPosition)
        {
            return path;
        }

        var connectingCells = GetConnectingCells(matrix, y, x)
            .Where(e => e.part is StartingPosition && path.Count > 2 || !path.Contains(e)); // Starting position only if closing loop and avoid already used cells.

        var bestPath = new List<(int y, int x, string part)>();
        foreach (var cell in connectingCells)
        {
            var pathCopy = path.Select(tuple => tuple).Append(cell).ToList();
            var foundPath = FindPath(matrix, cell.y, cell.x, pathCopy);

            // Found a better path...
            if (foundPath.Count <= path.Count)
            {
                continue;
            }

            bestPath = foundPath;
            break;
        }

        return bestPath;
    }

    private static void PrintMatrixFromPath(string[][] matrix, List<(int y, int x, string part)> path)
    {
        var matrixCopy = matrix.Select(e => e.Select(x => x).ToArray()).ToArray();
        for (int y = 0; y < matrixCopy.Length; y++)
        {
            for (int x = 0; x < matrixCopy[y].Length; x++)
            {
                var isInPath = path.Contains((y, x, matrixCopy[y][x]));
                if (!isInPath)
                {
                    matrixCopy[y][x] = Ground;
                }
            }
        }

        foreach (var row in matrixCopy)
        {
            Console.WriteLine(string.Join("", row));
        }
    }

    private List<(int y, int x, string part)> FindClosingPath(string[][] matrix, int y, int x, List<(int y, int x, string part)> path)
    {
        if (path.Count > 2 && path.First().part is StartingPosition && path.Last().part is StartingPosition)
        {
            return path;
        }

        var connectingPieces = GetConnectingCells(matrix, y, x);

        if (connectingPieces.Count == 0)
        {
            return path;
        }

        var paths = new List<List<(int y, int x, string part)>>();
        foreach (var connectingPiece in connectingPieces.Where(e => e.part is StartingPosition && path.Count > 2 || !path.Contains(e)))
        {
            // Avoid closing circle
            if (path.Count >= 2 && connectingPiece.part == StartingPosition && path.ElementAt(path.Count - 2).part is StartingPosition)
            {
                paths.Add(path);
                continue;
            }

            var pathCopy = path.Select(tuple => tuple).Append(connectingPiece).ToList();
            var foundPath = FindClosingPath(matrix, connectingPiece.y, connectingPiece.x, pathCopy);
            if (foundPath.Count == 0)
            {
                continue;
            }

            if (foundPath.Count > 2 && foundPath.First().part is StartingPosition && foundPath.Last().part is StartingPosition)
            {
                Console.WriteLine(string.Join(" -> ", foundPath));
                return foundPath;
            }

            if (foundPath.Count > path.Count)
            {
                paths.Add(foundPath);
                continue;
            }


            paths.Add(path);
        }

        var longestPath = paths.Count == 0
            ? new List<(int y, int x, string part)>()
            : paths.OrderBy(e => e.Count).First();
        return longestPath;
    }

    private static List<(int y, int x, string part)> GetConnectingCells(string[][] matrix, int y, int x)
    {
        var currentCell = matrix[y][x];
        var topCell = y == 0 ? Ground : matrix[y - 1][x];
        var bottomCell = y == matrix.Length - 1 ? Ground : matrix[y + 1][x];
        var leftCell = x == 0 ? Ground : matrix[y][x - 1];
        var rightCell = x == matrix[y].Length - 1 ? Ground : matrix[y][x + 1];

        var connectingPieces = new List<(int y, int x, string part)>();

        if (topCell is not Ground && AreCellsConnected(currentCell, topCell, Direction.South) && AreCellsConnected(topCell, currentCell, Direction.North))
        {
            connectingPieces.Add((y - 1, x, topCell));
        }

        if (bottomCell is not Ground && AreCellsConnected(currentCell, bottomCell, Direction.North) && AreCellsConnected(bottomCell, currentCell, Direction.South))
        {
            connectingPieces.Add((y + 1, x, bottomCell));
        }

        if (leftCell is not Ground && AreCellsConnected(currentCell, leftCell, Direction.East) && AreCellsConnected(leftCell, currentCell, Direction.West))
        {
            connectingPieces.Add((y, x - 1, leftCell));
        }

        if (rightCell is not Ground && AreCellsConnected(currentCell, rightCell, Direction.West) && AreCellsConnected(rightCell, currentCell, Direction.East))
        {
            connectingPieces.Add((y, x + 1, rightCell));
        }

        return connectingPieces;
    }

    internal enum Direction
    {
        North = 1,
        South,
        West,
        East
    }

    public static Dictionary<string, List<Direction>> Map = new()
    {
        // Eg: Vertical is connecting north and south.
        { Vertical, new() { Direction.North, Direction.South } },
        { Horizontal, new() { Direction.West, Direction.East } },
        { BottomLeftCorner, new() { Direction.North, Direction.East } },
        { BottomRightCorner, new() { Direction.North, Direction.West } },
        { TopLeftCorner, new() { Direction.South, Direction.East } },
        { TopRightCorner, new() { Direction.South, Direction.West } },
        { StartingPosition, new() { Direction.North, Direction.South, Direction.West, Direction.East } }
    };

    public static bool AreCellsConnected(string current, string next, Direction fromDirection)
    {
        var hasValue = Map.TryGetValue(next, out var allowedDirection);
        return hasValue && allowedDirection!.Contains(fromDirection);
    }

    // private static bool CanConnectTo(string current, string next, Direction direction)
    // {
    //     if (next is Ground)
    //     {
    //         return false;
    //     }
    //
    //     return current switch
    //     {
    //         StartingPosition when direction is Direction.North => next is Vertical or TopLeftCorner or TopRightCorner,
    //         StartingPosition when direction is Direction.South => next is Vertical or BottomLeftCorner or BottomRightCorner,
    //         StartingPosition when direction is Direction.West => next is Horizontal or BottomLeftCorner or TopLeftCorner,
    //         StartingPosition when direction is Direction.East => next is Horizontal or BottomRightCorner or TopRightCorner,
    //         StartingPosition => next is not Ground,
    //         Ground => false,
    //
    //         Vertical when direction is Direction.North => next is Vertical or TopLeftCorner or TopRightCorner or StartingPosition,
    //         Vertical when direction is Direction.South => next is Vertical or BottomLeftCorner or BottomRightCorner or StartingPosition,
    //         Vertical when direction is Direction.West => false,
    //         Vertical when direction is Direction.East => false,
    //
    //         Horizontal when direction is Direction.North => false,
    //         Horizontal when direction is Direction.South => false,
    //         Horizontal when direction is Direction.West => next is Horizontal or BottomLeftCorner or TopLeftCorner or StartingPosition,
    //         Horizontal when direction is Direction.East => next is Horizontal or TopRightCorner or BottomRightCorner or StartingPosition,
    //
    //         TopLeftCorner when direction is Direction.North => false,
    //         TopLeftCorner when direction is Direction.West => false,
    //         TopLeftCorner when direction is Direction.South => next is Vertical or BottomLeftCorner or BottomRightCorner or StartingPosition,
    //         TopLeftCorner when direction is Direction.East => next is Horizontal or BottomRightCorner or TopRightCorner or StartingPosition,
    //
    //         TopRightCorner when direction is Direction.North => false,
    //         TopRightCorner when direction is Direction.East => false,
    //         TopRightCorner when direction is Direction.West => next is Horizontal or BottomLeftCorner or TopLeftCorner or StartingPosition,
    //         TopRightCorner when direction is Direction.South => next is Vertical or BottomLeftCorner or BottomRightCorner or StartingPosition,
    //
    //         BottomLeftCorner when direction is Direction.West => false,
    //         BottomLeftCorner when direction is Direction.South => false,
    //         BottomLeftCorner when direction is Direction.North => next is Vertical or TopLeftCorner or TopRightCorner or StartingPosition,
    //         BottomLeftCorner when direction is Direction.East => next is Horizontal or TopRightCorner or BottomRightCorner or StartingPosition,
    //
    //         BottomRightCorner when direction is Direction.East => false,
    //         BottomRightCorner when direction is Direction.South => false,
    //         BottomRightCorner when direction is Direction.North => next is Vertical or TopLeftCorner or TopRightCorner or StartingPosition,
    //         BottomRightCorner when direction is Direction.West => next is Horizontal or TopLeftCorner or BottomLeftCorner or StartingPosition,
    //         _ => throw new ArgumentOutOfRangeException(nameof(current), current, null)
    //     };
    // }
}
