using System.Text.RegularExpressions;

var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

long part1Result = 0;
long part2Result = 0;

var regex = new Regex(@"(?<Springs>(\.|\#|\?)+).(?<Values>((?<Value>(\d)),?)*)");

await foreach (var line in File.ReadLinesAsync(filePath))
{
    var match = regex.Match(line);

    var springs = match.Groups["Springs"].Value;
    var values = match.Groups["Value"].Captures.Select(e => int.Parse(e.Value));

    // var lineRegex = new Regex($@".*(?=[\.\?]*[\#\?]{{1}}[\.\?]+).*(?=[\.\?]*[\#\?]{{1}}[\.\?]+).*([\.\?]*[\#\?]{{3}}[\.\?]+).*");
    var lineRegex = new Regex(@"(?<=\?)+(\.)(\#{3})");

    var result = lineRegex.Matches(line);

    var mimmo = "mammamimmo";
}

Console.WriteLine($"Part 1 result: {part1Result}");
