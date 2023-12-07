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
    // var notPartNumbers = new Regex(@"([\d|\.][\.][\d|\.])([\d|\.][\d][\d|\.])+([\d|\.][\.][\d|\.])");
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
