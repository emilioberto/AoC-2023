using System.Text;
using System.Text.RegularExpressions;

var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

long part1Result = 0;
long part2Result = 0;

var seedsPattern = @"seeds:(\s*(?<SeedNumber>(\d+)))+([\r\n]*)";
var destStartPattern = $@"(?<DestinationStart>(\d+)\s*)";
var srcStartPattern = $@"(?<SourceStart>(\d+)\s*)";
var rangePattern = $@"(?<Range>(\d+)\s*)";
var mapPattern = $@"(?<Map>((?<From>(.*))-to-(?<To>(.*)))\s+map:([\r\n]*))";
var mapRulesPattern = $@"(?<MapRules>{destStartPattern}{srcStartPattern}{rangePattern})+";
var mapsPattern = $@"(?<Maps>({mapPattern}{mapRulesPattern})([\r\n]*))+";

var almanacRegex = new Regex(@$"{seedsPattern}{mapsPattern}");
var input = await File.ReadAllTextAsync(filePath);

var match = almanacRegex.Match(input);

var seedNumbers = match.Groups["SeedNumber"].Captures.Select(e => long.Parse(e.Value)).ToArray();
var maps = match.Groups["Maps"].Captures.ToArray();
var mapFrom = match.Groups["From"].Captures.ToArray();
var mapTo = match.Groups["To"].Captures.ToArray();
var mapRules = match.Groups["MapRules"].Captures.ToArray();
var destinationStarts = match.Groups["DestinationStart"].Captures.ToArray();
var sourceStarts = match.Groups["SourceStart"].Captures.ToArray();
var ranges = match.Groups["Range"].Captures.ToArray();

var almanac = maps.Select((mapCapture, index) =>
{
    var map = new Map
    {
        From = mapFrom.First(e => e.Index >= mapCapture.Index).Value,
        To = mapTo.First(e => e.Index >= mapCapture.Index).Value,
    };

    var mapRulesCaptures = index == maps.Length - 1
        ? mapRules.Where(e => e.Index >= mapCapture.Index).ToArray()
        : mapRules.Where(e => e.Index >= mapCapture.Index && e.Index < maps[index + 1].Index).ToArray();

    foreach (var mapRuleCapture in mapRulesCaptures)
    {
        map.MapRules.Add(new MapRule
        {
            DestinationStart = long.Parse(destinationStarts.First(e => e.Index >= mapRuleCapture.Index).Value),
            SourceStart = long.Parse(sourceStarts.First(e => e.Index >= mapRuleCapture.Index).Value),
            Range = long.Parse(ranges.First(e => e.Index >= mapRuleCapture.Index).Value)
        });
    }

    return map;
}).ToArray();

var locationResults = seedNumbers.Select(seedNumber =>
{
    var conversionMap = almanac.Single(e => e.From == "seed");
    var result = seedNumber;

    while (conversionMap is not null)
    {
        var existingMapping = conversionMap.MapRules.SingleOrDefault(e => result >= e.SourceStart && result <= e.SourceStart + e.Range - 1);
        if (existingMapping is not null)
        {
            var offset = existingMapping.DestinationStart - existingMapping.SourceStart;
            result += offset;
        }

        conversionMap = almanac.SingleOrDefault(e => e.From == conversionMap.To);
    }

    return result;
}).ToArray();

part1Result = locationResults.Min();

Console.WriteLine($"Part 1 result: {part1Result}");

// ------
// PART 2
// ------

var seedNumbersRanges = new List<(long start, long end)>();
for (long i = 0; i < seedNumbers.Length - 1; i += 2)
{
    seedNumbersRanges.Add((seedNumbers[i], seedNumbers[i] + seedNumbers[i + 1] - 1));
}

var found = false;
part2Result = 0;
while (!found)
{
    long result = part2Result;
    var conversionMap = almanac.Single(e => e.To == "location");

    while (conversionMap is not null)
    {
        foreach (var mapRule in conversionMap.MapRules)
        {
            if (mapRule.DestinationStart <= result && result <= mapRule.DestinationStart + mapRule.Range - 1)
            {
                result -= (mapRule.DestinationStart - mapRule.SourceStart);
                break;
            }
        }

        conversionMap = almanac.SingleOrDefault(e => e.To == conversionMap.From);
    }

    foreach (var seedNumbersRange in seedNumbersRanges)
    {
        if (result >= seedNumbersRange.start && result <= seedNumbersRange.end)
        {
            found = true;
            break;
        }
    }

    if (!found)
    {
        part2Result++;
    }
}

Console.WriteLine($"Part 2 result: {part2Result}");

record Map
{
    public string From { get; set; }
    public string To { get; set; }

    public List<MapRule> MapRules { get; set; } = new();
}

record MapRule
{
    public long DestinationStart { get; set; }
    public long SourceStart { get; set; }
    public long Range { get; set; }
}
