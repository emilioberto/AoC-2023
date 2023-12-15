using System.Text.RegularExpressions;

var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

long part1Result = 0;
long part2Result = 0;

var matrix = Array.Empty<string[]>();
await foreach (var line in File.ReadLinesAsync(filePath))
{
    matrix = matrix.Append(line.ToCharArray().Select(e => e.ToString()).ToArray()).ToArray();
}

for (var y = 0; y < matrix.Length; y++)
{
    var row = matrix[y];


    for (var x = 0; x < row.Length; x++)
    {
        if (row[x] == ground)
        {
            continue;
        }

        var isNotConnectable = !CanConnectToSomeone(matrix, y, x);

        if (isNotConnectable)
        {
            matrix[y][x] = ".";
        }
    }
}


Console.WriteLine(matrix.Length);

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

    bool CanConnectToSomeone(string[][] matrix, int y, int x)
    {
        var currentCell = matrix[y][x];
        var topCell = y == 0 ? null : matrix[y - 1][x];
        var bottomCell = y == matrix.Length - 1 ? null : matrix[y + 1][x];
        var leftCell = x == 0 ? null : matrix[y][x - 1];
        var rightCell = x == matrix[y].Length - 1 ? null : matrix[y][x + 1];
        if (currentCell == ".")
        {
            return false;
        }

        if (topCell is "|")
        {
            return currentCell is Vertical or BottomLeftCorner or BottomRightCorner or StartingPosition;
        }
        if (bottomCell is "|")
        {
            return currentCell is Vertical or TopLeftCorner or TopRightCorner or StartingPosition;
        }
        if (topCell is "|")
        {
            return currentCell is Vertical or BottomLeftCorner or BottomRightCorner or StartingPosition;
        }
        if (topCell is "|")
        {
            return currentCell is Vertical or BottomLeftCorner or BottomRightCorner or StartingPosition;
        }

            return false;
    }
}
