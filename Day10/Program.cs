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

List<PathNode> nestCoordinates = new();
for (var y = 0; y < matrix.Length; y++)
{
    var row = matrix[y];
    for (var x = 0; x < row.Length; x++)
    {
        if (matrix[y][x] is not Ground)
        {
            continue;
        }

        List<string> leftElements = new();
        List<string> rightElements = new();
        for (int i = 0; i < row.Length; i++)
        {
            if (i < x && row[i] is not Ground)
            {
                leftElements.Add(row[i]);
            }
            else if (i < x && row[i] is Ground)
            {
                leftElements = new();
            }

            if (i > x && row[i] is not Ground)
            {
                rightElements.Add(row[i]);
            }
            else if (i > x && row[i] is Ground)
            {
                rightElements = new();
            }
        }
        if (leftElements.Count == 0 || rightElements.Count == 0 || (leftElements.Count + rightElements.Count) % 2 != 0)
        {
            continue;
        }

        List<string> topElements = new();
        List<string> bottomElements = new();
        for (int i = 0; i < matrix.Length; i++)
        {
            if (i < y && matrix[i][x] is not Ground)
            {
                topElements.Add(matrix[i][x]);
            }
            else if (i < y && matrix[i][x] is Ground)
            {
                topElements = new();
            }

            if (i > y && matrix[i][x] is not Ground)
            {
                bottomElements.Add(matrix[i][x]);
            }
            else if (i > y && matrix[i][x] is Ground)
            {
                bottomElements = new();
            }
        }

        if (topElements.Count == 0 || bottomElements.Count == 0 || (topElements.Count + bottomElements.Count) % 2 != 0)
        {
            continue;
        }

        nestCoordinates.Add(new(y, x, Nest));
    }
}

foreach (var coordinate in nestCoordinates)
{
    matrix[coordinate.Y][coordinate.X] = coordinate.Part;
}

var fakeNestsRemoved = -1;
while (fakeNestsRemoved != 0)
{
    fakeNestsRemoved = 0;
    for (var y = 0; y < matrix.Length; y++)
    {
        var row = matrix[y];
        for (var x = 0; x < row.Length; x++)
        {
            if (matrix[y][x] is not Nest)
            {
                continue;
            }

            var topCell = y == 0 ? null : matrix[y - 1][x];
            var bottomCell = y == matrix.Length - 1 ? null : matrix[y + 1][x];
            var leftCell = x == 0 ? null : matrix[y][x - 1];
            var rightCell = x == matrix[y].Length - 1 ? null : matrix[y][x + 1];
            if (topCell is null or Ground
                || bottomCell is null or Ground
                || leftCell is null or Ground
                || rightCell is null or Ground
               )
            {
                matrix[y][x] = Ground;
                fakeNestsRemoved++;
            }
        }
    }
}

foreach (var row in matrix)
{
    Console.WriteLine(string.Join("", row));
}

part2Result = matrix.SelectMany(row => row.Where(e => e is Nest)).Count();
Console.WriteLine($"Part 2 result: {part2Result}");

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
    public const string Nest = "O";

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
