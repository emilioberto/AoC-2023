var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

long part1Result = 0;
long part2Result = 0;

var matrix = Array.Empty<string[]>();
await foreach (var line in File.ReadLinesAsync(filePath))
{
    matrix = matrix.Append(line.ToCharArray().Select(e => e.ToString()).ToArray()).ToArray();
}

(int y, int x) start = (0, 0);
for (var y = 0; y < matrix.Length; y++)
{
    var row = matrix[y];
    for (var x = 0; x < row.Length; x++)
    {
        if (row[x] is not StartingPosition)
        {
            continue;
        }

        start = (y, x);
    }
}

var currentPosition = start;
List<PathNode> foundPath = new() { new PathNode(start.y, start.x, matrix[start.y][start.x]) };
while (foundPath.Count <= 3 || foundPath.Last().Part is not StartingPosition)
{
    var connectingCells = GetConnectingCells(matrix, currentPosition.y, currentPosition.x)
        .Where(node => !foundPath.Contains(node) || node.Part is StartingPosition && foundPath.Count > 3)
        .ToList();

    if (connectingCells.Count is 0)
    {
        foreach (var path in foundPath.Where((node, index) => index > 0))
        {
            matrix[path.Y][path.X] = Ground;
        }

        foundPath = new();
        currentPosition = start;
        continue;
    }

    var nextCell = connectingCells.First();
    foundPath.Add(nextCell);
    currentPosition = new(nextCell.Y, nextCell.X);
}

PrintMatrixFromPath(matrix, foundPath);
part1Result = (foundPath.Count - 1) / 2;

Console.WriteLine($"Part 1 result: {part1Result}");

// Part 2

var topLeftCorner = foundPath
    .Where(node => node.Part is TopLeftCorner)
    .OrderBy(e => e.Y)
    .ThenBy(node => node.X)
    .First();

var index = foundPath.IndexOf(topLeftCorner);
var foundPathStartingFromTopLeftCorner = foundPath.GetRange(index, foundPath.Count - index);
foundPathStartingFromTopLeftCorner.AddRange(foundPath.GetRange(0, index));

// Check if we are going to the right side or to bottom and set initial offsets
var yOffset = foundPathStartingFromTopLeftCorner[1].Part is Vertical ? 0 : -1;
var xOffset = foundPathStartingFromTopLeftCorner[1].Part is Vertical ? -1 : 0;
for (var i = 0; i < foundPathStartingFromTopLeftCorner.Count; i++)
{
    PathNode? previousNode = (i - 1 >= 0) ? foundPathStartingFromTopLeftCorner[i - 1] : null;
    var pathNode = foundPathStartingFromTopLeftCorner[i];
    var nextNode = i + 1 < foundPathStartingFromTopLeftCorner.Count
        ? foundPathStartingFromTopLeftCorner[i + 1]
        : foundPathStartingFromTopLeftCorner[0];

    var xCellToFix = pathNode.X;
    if (xOffset < 0)
    {
        xCellToFix = Math.Max(pathNode.X + xOffset, 0);
    }
    else if (xOffset > 0)
    {
        xCellToFix = Math.Min(pathNode.X + xOffset, matrix[0].Length - 1);
    }

    var yCellToFix = pathNode.Y;
    if (yOffset < 0)
    {
        yCellToFix = Math.Max(pathNode.Y + yOffset, 0);
    }
    else if (yOffset > 0)
    {
        yCellToFix = Math.Min(pathNode.Y + yOffset, matrix.Length - 1);
    }

    if (!foundPath.Any(node => node.Y == yCellToFix && node.X == xCellToFix))
    {
        matrix[yCellToFix][xCellToFix] = OutOfTheLoop;
    }

    if (pathNode.Y != nextNode.Y) // We are going left or right
    {
        var wasLastStepToRight = (previousNode.HasValue && previousNode.Value.X < pathNode.X);
        xOffset = wasLastStepToRight ? +1 : -1;
        yOffset = 0;
    }

    if (pathNode.X != nextNode.X) // We are going up or down
    {
        var wasLastStepToDown = (previousNode.HasValue && previousNode.Value.Y < pathNode.Y);
        yOffset = wasLastStepToDown ? +1 : -1;
        xOffset = 0;
    }

    Console.Clear();
    for (var y = 0; y < matrix.Length; y++)
    {
        var row = matrix[y];
        if (y == pathNode.Y)
        {
            row[pathNode.X] = "#";
        }

        Console.WriteLine(string.Join("", row.Select(s => s)));
    }
}

var hadChanges = true;
while (hadChanges)
{
    hadChanges = false;
    for (var y = 0; y < matrix.Length; y++)
    {
        for (int x = 0; x < matrix[y].Length; x++)
        {
            if (matrix[y][x] is not Ground)
            {
                continue;
            }

            var currentCell = matrix[y][x];
            var topCell = y == 0 ? Ground : matrix[y - 1][x];
            var bottomCell = y == matrix.Length - 1 ? Ground : matrix[y + 1][x];
            var leftCell = x == 0 ? Ground : matrix[y][x - 1];
            var rightCell = x == matrix[y].Length - 1 ? Ground : matrix[y][x + 1];

            if (
                topCell is OutOfTheLoop
                || bottomCell is OutOfTheLoop
                || leftCell is OutOfTheLoop
                || rightCell is OutOfTheLoop
            )
            {
                hadChanges = true;
                matrix[y][x] = OutOfTheLoop;
            }
        }
    }
}

Console.WriteLine("\n\n");

foreach (var row in matrix)
{
    Console.WriteLine(string.Join("", row.Select(s => s is Ground or OutOfTheLoop ? s : "=")));
}

part2Result = matrix.SelectMany(row => row.Where(e => e is Nest)).Count();
Console.WriteLine($"Part 2 result: {part2Result}");

bool IsInRowPair((PathNode? p1, PathNode? p2) pair, int x)
{
    if (pair.p1.HasValue && pair.p2.HasValue)
    {
        var isInPair = pair.p1!.Value.X < x && pair.p2!.Value.X > x;
        return isInPair;
    }

    if (pair.p1.HasValue && !pair.p2.HasValue)
    {
        return x < pair.p1.Value.X;
    }

    return false;
}

;

bool IsInColPair((PathNode? p1, PathNode? p2) pair, int y)
{
    if (pair.p1.HasValue && pair.p2.HasValue)
    {
        var isInPair = pair.p1.Value.Y < y && pair.p2.Value.Y > y;
        return isInPair;
    }

    if (pair.p1.HasValue && !pair.p2.HasValue)
    {
        return y < pair.p1.Value.Y;
    }

    return false;
}

;


internal struct PathNode
{
    public int Y { get; set; }
    public int X { get; set; }
    public string Part { get; set; }

    public PathNode(int y, int x, string part)
    {
        Y = y;
        X = x;
        Part = part;
    }
}

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
    public const string OutOfTheLoop = "O";
    public const string Nest = "I";

    private static void PrintMatrixFromPath(string[][] matrix, List<PathNode> path)
    {
        var matrixCopy = matrix.Select(e => e.Select(x => x).ToArray()).ToArray();
        for (int y = 0; y < matrixCopy.Length; y++)
        {
            for (int x = 0; x < matrixCopy[y].Length; x++)
            {
                var isInPath = path.Contains(new(y, x, matrixCopy[y][x]));
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

    private static List<PathNode> GetConnectingCells(string[][] matrix, int y, int x)
    {
        var currentCell = matrix[y][x];
        var topCell = y == 0 ? Ground : matrix[y - 1][x];
        var bottomCell = y == matrix.Length - 1 ? Ground : matrix[y + 1][x];
        var leftCell = x == 0 ? Ground : matrix[y][x - 1];
        var rightCell = x == matrix[y].Length - 1 ? Ground : matrix[y][x + 1];

        var connectingPieces = new List<PathNode>();

        if (topCell is not Ground && AreCellsConnected(currentCell, topCell, Direction.South) && AreCellsConnected(topCell, currentCell, Direction.North))
        {
            connectingPieces.Add(new(y - 1, x, topCell));
        }

        if (bottomCell is not Ground && AreCellsConnected(currentCell, bottomCell, Direction.North) && AreCellsConnected(bottomCell, currentCell, Direction.South))
        {
            connectingPieces.Add(new(y + 1, x, bottomCell));
        }

        if (leftCell is not Ground && AreCellsConnected(currentCell, leftCell, Direction.East) && AreCellsConnected(leftCell, currentCell, Direction.West))
        {
            connectingPieces.Add(new(y, x - 1, leftCell));
        }

        if (rightCell is not Ground && AreCellsConnected(currentCell, rightCell, Direction.West) && AreCellsConnected(rightCell, currentCell, Direction.East))
        {
            connectingPieces.Add(new(y, x + 1, rightCell));
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
}
