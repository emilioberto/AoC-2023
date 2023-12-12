using System.Text;
using System.Text.RegularExpressions;

var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

var part1Result = 0m;
var part2Result = 0m;

var matrix = Array.Empty<string[]>();
await foreach (var line in File.ReadLinesAsync(filePath))
{
    var row = line.ToCharArray().Select(e => e.ToString()).ToArray();
    matrix = matrix.Append(row).ToArray();
}


for (var i = 0; i < matrix.Length; i++)
{
    var currentRow = matrix[i];
    var previousRow = i == 0 ? matrix[i].Select(e => ".").ToArray() : matrix[i - 1];
    var nextRow = i == matrix.Length - 1 ? matrix[i].Select(e => ".").ToArray() : matrix[i + 1];

    var isDigit = new Regex($@"\d");
    var allNumbers = new Regex(@"\d+");
    var notPartNumbers = new Regex(@"(?<=([\d|\.][\.][\d|\.]))([\d|\.][\d][\d|\.])+(?=([\d|\.][\.][\d|\.]))");

    var builder = new StringBuilder().Append("...");
    for (int j = 0; j < currentRow.Length; j++)
    {
        builder
            .Append(isDigit.IsMatch(previousRow[j]) ? "." : previousRow[j])
            .Append(currentRow[j])
            .Append(isDigit.IsMatch(nextRow[j]) ? "." : nextRow[j]);
    }

    builder.Append("...");

    var mergedRowString = builder.ToString();
    Console.WriteLine(mergedRowString);

    var allNumberMatches = allNumbers.Matches(string.Join("", currentRow));
    foreach (Match match in allNumberMatches)
    {
        // Console.WriteLine($"Aggiungo {match.Value}");
        var matchStringValue = new Regex(@"\D").Replace(match.Value, "");
        part1Result += long.Parse(matchStringValue);
    }

    var matches = notPartNumbers.Matches(mergedRowString);
    foreach (Match match in matches)
    {
        var matchStringValue = new Regex(@"\D").Replace(match.Value, "");
        // Console.WriteLine($"Sottraggo {matchStringValue}");
        part1Result -= long.Parse(matchStringValue);
    }
}

Console.WriteLine($"Part 1 result: {part1Result}");

var matrixWithPadding = Array.Empty<string[]>();
var rowLength = matrix[0].Length;
var emptyRow = Enumerable.Range(0, rowLength).Select(e => ".").ToArray();
matrixWithPadding = matrixWithPadding.Append(emptyRow).ToArray();
foreach (var row in matrix)
{
    var paddedRow = Array.Empty<string>().Append(".").Concat(row).Append(".").ToArray();
    matrixWithPadding = matrixWithPadding.Append(paddedRow).ToArray();
}

matrixWithPadding = matrixWithPadding.Append(emptyRow).ToArray();

var numbersRegex = new Regex(@"\d+");
var digitOrDotRegex = new Regex(@"[^\d\.]");

var partNumbersWithGears = new List<PartNumberWithGear>();

for (int y = 1; y < matrixWithPadding.Length - 1; y++)
{
    var rowString = string.Join("", matrixWithPadding[y]);
    var matches = numbersRegex.Matches(rowString);
    if (!matches.Any())
    {
        continue;
    }

    foreach (var match in matches.ToArray())
    {
        var x = match.Index;
        var length = match.Length;

        for (int i = x; i < x + length; i++)
        {
            var top = digitOrDotRegex.Match(matrixWithPadding[y - 1][i]);
            var topLeft = digitOrDotRegex.Match(matrixWithPadding[y - 1][i - 1]);
            var topRight = digitOrDotRegex.Match(matrixWithPadding[y - 1][i + 1]);
            var bottom = digitOrDotRegex.Match(matrixWithPadding[y + 1][i]);
            var bottomLeft = digitOrDotRegex.Match(matrixWithPadding[y + 1][i - 1]);
            var bottomRight = digitOrDotRegex.Match(matrixWithPadding[y + 1][i + 1]);
            var left = digitOrDotRegex.Match(matrixWithPadding[y][i - 1]);
            var right = digitOrDotRegex.Match(matrixWithPadding[y][i + 1]);
            if (!top.Success
                && !topLeft.Success
                && !topRight.Success
                && !bottom.Success
                && !bottomLeft.Success
                && !bottomRight.Success
                && !left.Success
                && !right.Success)
            {
                continue;
            }

            var partNumberWithGear = new PartNumberWithGear
            {
                Number = long.Parse(match.Value)
            };

            if (top.Value is "*")
            {
                partNumberWithGear.GearCoordinate = (y - 1, i);
            }

            if (topLeft.Value is "*")
            {
                partNumberWithGear.GearCoordinate = (y - 1, i - 1);
            }

            if (topRight.Value is "*")
            {
                partNumberWithGear.GearCoordinate = (y - 1, i + 1);
            }

            if (bottom.Value is "*")
            {
                partNumberWithGear.GearCoordinate = (y + 1, i);
            }

            if (bottomLeft.Value is "*")
            {
                partNumberWithGear.GearCoordinate = (y + 1, i - 1);
            }

            if (bottomRight.Value is "*")
            {
                partNumberWithGear.GearCoordinate = (y + 1, i + 1);
            }

            if (left.Value is "*")
            {
                partNumberWithGear.GearCoordinate = (y, i - 1);
            }

            if (right.Value is "*")
            {
                partNumberWithGear.GearCoordinate = (y, i + 1);
            }

            if (partNumbersWithGears.Any(e => e.GearCoordinate == partNumberWithGear.GearCoordinate && e.Number == partNumberWithGear.Number))
            {
                continue;
            }
            partNumbersWithGears.Add(partNumberWithGear);
        }
    }
}

var groupedPartNumbersWithGears = partNumbersWithGears
    .GroupBy(e => e.GearCoordinate)
    .Where(e => e.Count() is 2);
foreach (var group in groupedPartNumbersWithGears)
{
    var numbers = group.Select(e => e.Number).ToArray();
    part2Result += (numbers[0] * numbers[1]);
}


Console.WriteLine($"Part 2 result: {part2Result}");


record PartNumberWithGear
{
    public (long y, long x) GearCoordinate { get; set; }
    public long Number { get; set; }
}
