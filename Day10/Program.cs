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

if (start.y <= topLeftCorner.Y && start.x <= topLeftCorner.X)
{
    topLeftCorner = foundPath.First(node => node.Part is StartingPosition);
}

var index = foundPath.IndexOf(topLeftCorner);
var foundPathStartingFromTopLeftCorner = foundPath.GetRange(index, foundPath.Count - index);
foundPathStartingFromTopLeftCorner.AddRange(foundPath.GetRange(0, index));

// Check if we are going to the right side or to bottom and set initial offset
(int y, int x) offset = foundPathStartingFromTopLeftCorner[1].Part is Vertical ? (0, 1) : (1, 0);

var lastCorner = TopLeftCorner;
var direction = foundPathStartingFromTopLeftCorner[1].Part is Vertical ? Vertical : Horizontal;

for (var i = 0; i < foundPathStartingFromTopLeftCorner.Count; i++)
{
    var currentNode = foundPathStartingFromTopLeftCorner[i];
    switch (lastCorner)
    {
        case TopLeftCorner:
            offset = direction is Vertical ? (0, 1) : (1, 0);
            break;
        case TopRightCorner:
            offset = direction is Vertical ? (0, -1) : (1, 0);
            break;
        case BottomLeftCorner:
            offset = direction is Vertical ? (0, 1) : (-1, 0);
            break;
        case BottomRightCorner:
            offset = direction is Vertical ? (0, -1) : (-1, 0);
            break;
    }

    var nodeToCheck = foundPathStartingFromTopLeftCorner[i];
    nodeToCheck.Y += offset.y;
    nodeToCheck.X += offset.x;

    if (!foundPath.Any(node => node.Y == nodeToCheck.Y + offset.y && node.X == nodeToCheck.X)) { }

    matrix[nodeToCheck.Y + offset.y][nodeToCheck.X + offset.x] = OutOfTheLoop;

    if (currentNode.Part is TopLeftCorner or TopRightCorner or BottomLeftCorner or BottomRightCorner or StartingPosition)
    {
        switch (lastCorner)
        {
            case TopLeftCorner:
                direction = direction is Vertical ? Horizontal : Vertical;
                break;
            case TopRightCorner:
                direction = direction is Vertical ? Horizontal : Vertical;
                break;
            case BottomLeftCorner:
                direction = direction is Vertical ? Horizontal : Vertical;
                break;
            case BottomRightCorner:
                direction = direction is Vertical ? Horizontal : Vertical;
                break;
        }
    }


    Console.Clear();
    PrintMatrix(matrix);
}


//
//
// var corners = foundPathStartingFromTopLeftCorner.Where(node => node.Part is TopLeftCorner or TopRightCorner or BottomLeftCorner or BottomRightCorner or StartingPosition).ToList();
// (int y, int x) nodeToChange = (foundPathStartingFromTopLeftCorner[0].Y + offset.y, foundPathStartingFromTopLeftCorner[0].X + offset.x);
// for (var i = 0; i < foundPathStartingFromTopLeftCorner.Count - 1; i++)
// {
//     if (foundPathStartingFromTopLeftCorner[i].Part is not (TopLeftCorner or TopRightCorner or BottomLeftCorner or BottomRightCorner or StartingPosition))
//     {
//         continue;
//     }
//
//     (int y, int x) difference = (
//         foundPathStartingFromTopLeftCorner[i+1].Y - foundPathStartingFromTopLeftCorner[i].Y,
//         foundPathStartingFromTopLeftCorner[i+1].X - foundPathStartingFromTopLeftCorner[i].X
//     );
//
//     var j = i;
//     var requiredSteps = 2;
//     while (foundPathStartingFromTopLeftCorner[j].Part is not (TopLeftCorner or TopRightCorner or BottomLeftCorner or BottomRightCorner))
//     {
//         j++;
//         requiredSteps++;
//     }
//
//     var currentNode = foundPathStartingFromTopLeftCorner[i];
//     for (int step = 0; step < requiredSteps; step++)
//     {
//         if (step > 0)
//         {
//             nodeToChange.y += difference.y;
//             nodeToChange.x += difference.x;
//         }
//         if (!foundPath.Any(node => node.Y == nodeToChange.y && node.X == nodeToChange.x))
//         {
//             matrix[nodeToChange.y][nodeToChange.x] = OutOfTheLoop;
//         }
//
//         Console.Clear();
//         PrintMatrix(matrix);
//     }
//
//
//     //
//     // if (!foundPath.Any(node => node.Y == nodeToChange.y && node.X == nodeToChange.x))
//     // {
//     //     matrix[nodeToChange.y][nodeToChange.x] = OutOfTheLoop;
//     // }
//     // Console.Clear();
//     // PrintMatrix(matrix);
//     //
//     // currentNode = foundPathStartingFromTopLeftCorner[i + 1];
//     // nodeToChange.y = currentNode.Y + offset.y;
//     // nodeToChange.x = currentNode.X + offset.x;
// }

Console.WriteLine("\n\n");

foreach (var row in matrix)
{
    Console.WriteLine(string.Join("", row));
}

part2Result = matrix.SelectMany(row => row.Where(e => e is Nest)).Count();
Console.WriteLine($"Part 2 result: {part2Result}");
;

void PrintMatrix(string[][] strings)
{
    for (var y = 0; y < strings.Length; y++)
    {
        var row = strings[y];
        Console.WriteLine(string.Join("", row.Select(s => s)));
    }
}


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

    internal enum FollowMode
    {
        Internal = 1,
        External = 2
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
