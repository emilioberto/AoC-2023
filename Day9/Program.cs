using System.Text.RegularExpressions;

var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

var historyRegex = new Regex(@"\-?\d+");
var part1Result = 0m;
var part2Result = 0m;
await foreach (var line in File.ReadLinesAsync(filePath))
{
    var matches = historyRegex.Matches(line);
    var numbers = matches.Select(e => e.Value).Select(long.Parse).ToList();

    var part1Lines = new List<long>();
    var part2Lines = new List<long>();
    CalculateResult(numbers, part1Lines, part2Lines);

    part1Result += numbers.Last() + part1Lines.Sum();

    var part2PartialResult = 0m;
    for (int i = part2Lines.Count() - 1; i >= 0; i--)
    {
        part2PartialResult = (part2Lines[i] - part2PartialResult);
    }
    part2Result += numbers.First() - part2PartialResult;
}

Console.WriteLine($"Part 1 result: {part1Result}");
Console.WriteLine($"Part 2 result: {part2Result}");

return;

void CalculateResult(List<long> input, List<long> part1Results, List<long> part2Results)
{
    Console.WriteLine(string.Join(" ", input));
    if (input.Count() == 1)
    {
        part1Results.Add(input.Last());
        part2Results.Add(input.First());
        return;
    }

    var differences = new List<long>();
    for (var i = 0; i < input.Count() - 1; i++)
    {
        differences.Add(input[i + 1] - input[i]);
    }

    if (differences.Distinct().Count() == 1)
    {
        Console.WriteLine(string.Join(" ", differences));
        part1Results.Add(differences.Last());
        part2Results.Add(differences.First());
        return;
    }

    part1Results.Add(differences.Last());
    part2Results.Add(differences.First());
    CalculateResult(differences, part1Results, part2Results);
}
