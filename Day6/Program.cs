using System.Text.RegularExpressions;

var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

long part1Result = 0;
long part2Result = 0;

var part1Regex = new Regex(@"Time:\s*(?<Time>(\d+)\s+)+[\r\n]*Distance:\s*(?<Distance>(\d+)\s+)+");

var match = part1Regex.Match(await File.ReadAllTextAsync(filePath));
var times = match.Groups["Time"].Captures.Select(e => long.Parse(e.Value)).ToArray();
var distances = match.Groups["Distance"].Captures.Select(e => long.Parse(e.Value)).ToArray();

for (int i = 0; i < times.Length; i++)
{
    var raceResults = new List<long>();
    var time = times[i];
    var distanceToBeat = distances[i];

    for (int j = 0; j < time; j++)
    {
        var speed = 1 * j;
        var travelDistance = speed * (time - j);
        if (travelDistance > distanceToBeat)
        {
            raceResults.Add(travelDistance);
        }
    }

    part1Result = part1Result == 0
        ? raceResults.Count
        : part1Result * raceResults.Count;
}

Console.WriteLine($"Part 1 result: {part1Result}");

var part2Time = long.Parse(string.Join("", times.Select(e => e.ToString())));
var part2Distance = long.Parse(string.Join("", distances.Select(e => e.ToString())));

var hugeRaceResults = new List<long>();

for (int j = 0; j < part2Time; j++)
{
    var speed = 1 * j;
    var travelDistance = speed * (part2Time - j);
    if (travelDistance > part2Distance)
    {
        hugeRaceResults.Add(travelDistance);
    }
}

part2Result = part2Result == 0
    ? hugeRaceResults.Count
    : part2Result * hugeRaceResults.Count;

Console.WriteLine($"Part 2 result: {part2Result}");
