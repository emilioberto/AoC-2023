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
    var connectingNodes = GetConnectingNodes(matrix, currentPosition.y, currentPosition.x)
        .Where(node => !foundPath.Contains(node) || node.Part is StartingPosition && foundPath.Count > 3)
        .ToList();

    if (connectingNodes.Count is 0)
    {
        foreach (var path in foundPath.Where((node, index) => index > 0))
        {
            matrix[path.Y][path.X] = Ground;
        }

        foundPath = new();
        currentPosition = start;
        continue;
    }

    var nextNode = connectingNodes.First();
    foundPath.Add(nextNode);
    currentPosition = new(nextNode.Y, nextNode.X);
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

var direction = foundPathStartingFromTopLeftCorner[1].Part is Vertical ? Vertical : Horizontal;

for (var i = 0; i < foundPathStartingFromTopLeftCorner.Count; i++)
{
    var currentNode = foundPathStartingFromTopLeftCorner[i];

    var nodeToCheck = foundPathStartingFromTopLeftCorner[i];

    if (i is not 0 && currentNode.Part is TopLeftCorner or TopRightCorner or BottomLeftCorner or BottomRightCorner)
    {

        if (
            nodeToCheck.X + offset.x < matrix[0].Length
            && nodeToCheck.X + offset.x >= 0
            && nodeToCheck.Y + offset.y < matrix.Length
            && nodeToCheck.Y + offset.y >= 0
        )
        {
            if (!foundPath.Any(node => node.Y == nodeToCheck.Y + offset.y && node.X == nodeToCheck.X + offset.x))
            {
                matrix[nodeToCheck.Y + offset.y][nodeToCheck.X + offset.x] = Nest;
            }
        }


        if (direction is Vertical)
        {
            direction = Horizontal;
            switch (currentNode.Part)
            {
                case TopLeftCorner:
                    offset = offset.x is -1 ? (-1, 0) : (1, 0);
                    break;
                case TopRightCorner:
                    offset = offset.x is -1 ? (1, 0) : (-1, 0);
                    break;
                case BottomLeftCorner:
                    offset = offset.x is -1 ? (1, 0) : (-1, 0);
                    break;
                case BottomRightCorner:
                    offset = offset.x is -1 ? (-1, 0) : (1, 0);
                    break;
            }
        }
        else
        {
            direction = Vertical;
            switch (currentNode.Part)
            {
                case TopLeftCorner:
                    offset = offset.y is -1 ? (0, -1) : (0, 1);
                    break;
                case TopRightCorner:
                    offset = offset.y is -1 ? (0, 1) : (0, -1);
                    break;
                case BottomLeftCorner:
                    offset = offset.y is -1 ? (0, 1) : (0, -1);
                    break;
                case BottomRightCorner:
                    offset = offset.y is -1 ? (0, -1) : (0, 1);
                    break;
            }
        }
    }

    nodeToCheck.Y += offset.y;
    nodeToCheck.X += offset.x;

    if (
        nodeToCheck.X >= matrix[0].Length
        || nodeToCheck.X < 0
        || nodeToCheck.Y >= matrix.Length
        || nodeToCheck.Y < 0
    )
    {
        continue;
    }

    if (!foundPath.Any(node => node.Y == nodeToCheck.Y && node.X == nodeToCheck.X))
    {
        matrix[nodeToCheck.Y][nodeToCheck.X] = Nest;
    }

    Console.Clear();
    PrintMatrix(matrix);
}

var hadChanges = true;
while (hadChanges)
{
    hadChanges = false;

    for (int y = 0; y < matrix.Length; y++)
    {
        for (int x = 0; x < matrix[y].Length; x++)
        {
            var currentNode = matrix[y][x];
            var topNode = y == 0 ? Ground : matrix[y - 1][x];
            var bottomNode = y == matrix.Length - 1 ? Ground : matrix[y + 1][x];
            var leftNode = x == 0 ? Ground : matrix[y][x - 1];
            var rightNode = x == matrix[y].Length - 1 ? Ground : matrix[y][x + 1];

            if (
                !foundPath.Any(node => node.Y == y && node.X == x) &&
                (topNode is Nest || bottomNode is Nest || leftNode is Nest || rightNode is Nest)
                && matrix[y][x] != Nest
            )
            {
                matrix[y][x] = Nest;
                hadChanges = true;
            }
        }
    }
}

Console.Clear();
PrintMatrix(matrix);

part2Result = matrix.SelectMany(row => row.Where(e => e is Nest)).Count();
Console.WriteLine($"Part 2 result: {part2Result}");

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

    private static List<PathNode> GetConnectingNodes(string[][] matrix, int y, int x)
    {
        var currentNode = matrix[y][x];
        var topNode = y == 0 ? Ground : matrix[y - 1][x];
        var bottomNode = y == matrix.Length - 1 ? Ground : matrix[y + 1][x];
        var leftNode = x == 0 ? Ground : matrix[y][x - 1];
        var rightNode = x == matrix[y].Length - 1 ? Ground : matrix[y][x + 1];

        var connectingPieces = new List<PathNode>();

        if (topNode is not Ground && AreNodesConnected(currentNode, topNode, Direction.South) && AreNodesConnected(topNode, currentNode, Direction.North))
        {
            connectingPieces.Add(new(y - 1, x, topNode));
        }

        if (bottomNode is not Ground && AreNodesConnected(currentNode, bottomNode, Direction.North) && AreNodesConnected(bottomNode, currentNode, Direction.South))
        {
            connectingPieces.Add(new(y + 1, x, bottomNode));
        }

        if (leftNode is not Ground && AreNodesConnected(currentNode, leftNode, Direction.East) && AreNodesConnected(leftNode, currentNode, Direction.West))
        {
            connectingPieces.Add(new(y, x - 1, leftNode));
        }

        if (rightNode is not Ground && AreNodesConnected(currentNode, rightNode, Direction.West) && AreNodesConnected(rightNode, currentNode, Direction.East))
        {
            connectingPieces.Add(new(y, x + 1, rightNode));
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

    public static bool AreNodesConnected(string current, string next, Direction fromDirection)
    {
        var hasValue = Map.TryGetValue(next, out var allowedDirection);
        return hasValue && allowedDirection!.Contains(fromDirection);
    }
}
