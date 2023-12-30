using System.Text;
using System.Threading.Channels;

var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

long part1Result = 0;
long part2Result = 0;

var matrix = new List<List<string>>();
await foreach (var line in File.ReadLinesAsync(filePath))
{
    var row = line.ToCharArray().Select(e => e.ToString()).ToList();
    matrix.Add(row);
}

var history = new Dictionary<(int y1, int x1, int y2, int x2), bool>();
var resultMatrix = CopyMatrix(matrix);

var firstCell = new Coordinate(0, 0);
var secondCell = matrix[0][0] is "|" or "\\" ? new Coordinate(1, 0) : new Coordinate(0, 1);

FindPath(matrix, resultMatrix, firstCell, secondCell, new List<Coordinate>());

part1Result = resultMatrix.SelectMany(row => row.Select(cell => cell)).Count(cellValue => cellValue is "#");
Console.WriteLine($"Part 1 result is {part1Result}");

var startingPoints = FindStartingPointsTuples(matrix);
var part2Results = new List<(Coordinate StartingPoint, int Result)>();
foreach (var startingPoint in startingPoints)
{
    history = new Dictionary<(int y1, int x1, int y2, int x2), bool>();
    resultMatrix = CopyMatrix(matrix);
    FindPath(matrix, resultMatrix, startingPoint.First, startingPoint.Second, new List<Coordinate>());
    var result = resultMatrix.SelectMany(row => row.Select(cell => cell)).Count(cellValue => cellValue is "#");
    part2Results.Add((startingPoint.First, result));
}


part2Result = part2Results.Max(e => e.Result);
Console.WriteLine($"Part 2 result is {part2Result}");


List<(Coordinate First, Coordinate Second)>? FindStartingPointsTuples(List<List<string>> matrix)
{
    var result = new List<(Coordinate First, Coordinate Second)>();

    // First Row
    for (int i = 0; i < matrix[0].Count; i++)
    {
        if (matrix[0][i] is "\\")
        {
            result.Add((new Coordinate(0, i), new Coordinate(0, i + 1)));
        }
        else if (matrix[0][i] is "/")
        {
            result.Add((new Coordinate(0, i), new Coordinate(0, i - 1)));
        }
        else if (matrix[0][i] is "-")
        {
            result.Add((new Coordinate(0, i), new Coordinate(0, i - 1)));
            result.Add((new Coordinate(0, i), new Coordinate(0, i + 1)));
        }
        else
        {
            result.Add((new Coordinate(0, i), new Coordinate(1, i)));
        }
    }

    // Left column
    for (int y = 0; y < matrix.Count; y++)
    {
        if (matrix[y][0] is "|")
        {
            result.Add((new Coordinate(y, 0), new Coordinate(y - 1, 0)));
            result.Add((new Coordinate(y, 0), new Coordinate(y + 1, 0)));
        }
        else if (matrix[y][0] is "\\")
        {
            result.Add((new Coordinate(y, 0), new Coordinate(y + 1, 0)));
        }
        else if (matrix[y][0] is "/")
        {
            result.Add((new Coordinate(y, 0), new Coordinate(y - 1, 0)));
        }
        else
        {
            result.Add((new Coordinate(y, 0), new Coordinate(y, 1)));
        }
    }

    // Right column
    for (int y = 0; y < matrix.Count; y++)
    {
        var lastColIndex = matrix[0].Count - 1;
        if (matrix[y][0] is "|")
        {
            result.Add((new Coordinate(y, lastColIndex), new Coordinate(y - 1, lastColIndex)));
            result.Add((new Coordinate(y, lastColIndex), new Coordinate(y + 1, lastColIndex)));
        }
        else if (matrix[y][0] is "\\")
        {
            result.Add((new Coordinate(y, lastColIndex), new Coordinate(y -1, lastColIndex)));
        }
        else if (matrix[y][0] is "/")
        {
            result.Add((new Coordinate(y, lastColIndex), new Coordinate(y + 1, lastColIndex)));
        }
        else
        {
            result.Add((new Coordinate(y, lastColIndex), new Coordinate(y, lastColIndex -1)));
        }
    }

    // Last row
    for (int i = 0; i < matrix[0].Count; i++)
    {
        var lastRowIndex = matrix.Count - 1;
        if (matrix[lastRowIndex][i] is "\\")
        {
            result.Add((new Coordinate(lastRowIndex, i), new Coordinate(lastRowIndex, i - 1)));
        }
        else if (matrix[0][i] is "/")
        {
            result.Add((new Coordinate(lastRowIndex, i), new Coordinate(lastRowIndex, i + 1)));
        }
        else if (matrix[0][i] is "-")
        {
            result.Add((new Coordinate(lastRowIndex, i), new Coordinate(lastRowIndex, i - 1)));
            result.Add((new Coordinate(lastRowIndex, i), new Coordinate(lastRowIndex, i + 1)));
        }
        else
        {
            result.Add((new Coordinate(lastRowIndex, i), new Coordinate(lastRowIndex - 1, i)));
        }
    }

    return result;
}

List<Coordinate> FindPath(List<List<string>> matrix, List<List<string>> resultMatrix, Coordinate lastCoordinate, Coordinate currentCoordinate, List<Coordinate> foundPath)
{
    history.Add((lastCoordinate.Y, lastCoordinate.X, currentCoordinate.Y, currentCoordinate.X), true);

    var isSplitted = false;
    var isOutOfBound = false;

    resultMatrix[lastCoordinate.Y][lastCoordinate.X] = "#";

    var currentCellValue = "";
    var difference = new Coordinate(currentCoordinate.Y - lastCoordinate.Y, currentCoordinate.X - lastCoordinate.X);
    var last = lastCoordinate;
    while (!isSplitted && !isOutOfBound)
    {
        Console.WriteLine(currentCoordinate);
        if (
            currentCoordinate.Y > matrix.Count - 1
            || currentCoordinate.X > matrix[0].Count - 1
            || currentCoordinate.X < 0
            || currentCoordinate.Y < 0
        )
        {
            isOutOfBound = true;
            continue;
        }


        difference = new Coordinate(currentCoordinate.Y - last.Y, currentCoordinate.X - last.X);
        resultMatrix[currentCoordinate.Y][currentCoordinate.X] = "#";
        last = currentCoordinate;

        currentCellValue = matrix[currentCoordinate.Y][currentCoordinate.X];
        switch (currentCellValue)
        {
            case "-" when difference.Y is not 0:
            case "|" when difference.Y is 0:
                isSplitted = true;
                break;
            case "\\" when difference.Y is 0:
                currentCoordinate = difference.X == 1
                    ? currentCoordinate with { Y = currentCoordinate.Y + 1 }
                    : currentCoordinate with { Y = currentCoordinate.Y - 1 };
                break;
            case "\\" when difference.Y is -1:
                currentCoordinate = currentCoordinate with { X = currentCoordinate.X - 1 };
                break;
            case "\\" when difference.Y is 1:
                currentCoordinate = currentCoordinate with { X = currentCoordinate.X + 1 };
                break;
            case "/" when difference.Y is 0:
                currentCoordinate = difference.X == 1
                    ? currentCoordinate with { Y = currentCoordinate.Y - 1 }
                    : currentCoordinate with { Y = currentCoordinate.Y + 1 };
                break;
            case "/" when difference.Y is -1:
                currentCoordinate = currentCoordinate with { X = currentCoordinate.X + 1 };
                break;
            case "/" when difference.Y is 1:
                currentCoordinate = currentCoordinate with { X = currentCoordinate.X - 1 };
                break;
            default:
                currentCoordinate = new Coordinate(currentCoordinate.Y + difference.Y, currentCoordinate.X + difference.X);
                break;
        }
    }

    if (isSplitted)
    {
        if (currentCellValue is "-" && difference.Y is not 0)
        {
            if (!history.TryGetValue((currentCoordinate.Y, currentCoordinate.X, currentCoordinate.Y, currentCoordinate.X - 1), out _))
            {
                FindPath(matrix, resultMatrix, currentCoordinate, currentCoordinate with { X = currentCoordinate.X - 1 }, foundPath);
            }

            if (!history.TryGetValue((currentCoordinate.Y, currentCoordinate.X, currentCoordinate.Y, currentCoordinate.X + 1), out _))
            {
                FindPath(matrix, resultMatrix, currentCoordinate, currentCoordinate with { X = currentCoordinate.X + 1 }, foundPath);
            }
        }

        if (currentCellValue is "|" && difference.Y is 0)
        {
            if (!history.TryGetValue((currentCoordinate.Y, currentCoordinate.X, currentCoordinate.Y - 1, currentCoordinate.X), out _))
            {
                FindPath(matrix, resultMatrix, currentCoordinate, currentCoordinate with { Y = currentCoordinate.Y - 1 }, foundPath);
            }

            if (!history.TryGetValue((currentCoordinate.Y, currentCoordinate.X, currentCoordinate.Y + 1, currentCoordinate.X), out _))
            {
                FindPath(matrix, resultMatrix, currentCoordinate, currentCoordinate with { Y = currentCoordinate.Y + 1 }, foundPath);
            }
        }
    }

    // We then return the resutls and will follow them to mark all the energized cells...

    return foundPath;
}


static List<List<string>> CopyMatrix(List<List<string>> matrix1)
{
    return matrix1.Select(list => list.Select(e => e).ToList()).ToList();
}
//
// static List<Coordinate> CopyList(List<Coordinate> list) =>
//     list.Select(coordinate => new Coordinate(coordinate.Y, coordinate.X)).ToList();

static void PrintMatrix(List<List<string>> strings)
{
    foreach (var row in strings)
    {
        Console.WriteLine(string.Join("", row.Select(e => e)));
    }
}

public record Coordinate(int Y, int X);
