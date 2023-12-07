using System.Text.RegularExpressions;

var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
var part1Result = 0m;
await foreach (var line in File.ReadLinesAsync(filePath))
{
    var matchesP1 = Regex.Matches(line, @"\d");

    var p1First = matchesP1.First().Value;
    var p1Last = matchesP1.LastOrDefault()?.Value ?? p1First;
    part1Result += long.Parse(ParseNumber(p1First) + ParseNumber(p1Last));
}
Console.WriteLine($"Part 1: {part1Result}");

var part2Result = 0m;
await foreach (var line in File.ReadLinesAsync(filePath))
{
    var matchesP2 = Regex.Matches(line, @"(?=(\d|one|two|three|four|five|six|seven|eight|nine))");
    var p2First = matchesP2.First().Groups[1].Value;
    var p2Last = matchesP2.LastOrDefault()?.Groups[1].Value ?? p2First;
    part2Result += long.Parse(ParseNumber(p2First) + ParseNumber(p2Last));
}

Console.WriteLine($"Part 2: {part2Result}");
return;

static string ParseNumber(string n)
{
    if (long.TryParse(n, out var result))
    {
        return result.ToString();
    }

    var map = new Dictionary<string, long>
    {
        { "one", 1 },
        { "two", 2 },
        { "three", 3 },
        { "four", 4 },
        { "five", 5 },
        { "six", 6 },
        { "seven", 7 },
        { "eight", 8 },
        { "nine", 9 }
    };

    map.TryGetValue(n, out var fromStringResult);
    return fromStringResult.ToString();
}

