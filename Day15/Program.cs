using System.Text;
using System.Text.RegularExpressions;

var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

long part1Result = 0;
long part2Result = 0;
var lines = File.ReadLines(filePath);

var partsRegex = new Regex(@"(?<Part>([^,]+))");

var matches = partsRegex.Matches(lines.First());


var parts = matches.Select(e => e.Groups["Part"]).Select(e => e.Value);

foreach (var part in parts)
{
    part1Result += CalculateHash(part, part1Result);
}

Console.WriteLine($"Part 1 result: {part1Result}");

// Part 2

var equalSignRegex = new Regex(@"((?<FirstPart>(\w)+)=((?<Digit>(\d))))");
var dashSignRegex = new Regex(@"(?<FirstPart>(\w)+)-");

var boxes = new List<List<Box>>();
for (int i = 0; i < 256; i++)
{
    boxes.Add(new List<Box>());
}

foreach (var part in parts)
{
    var equalMatch = equalSignRegex.Match(part);
    var dashMatch = dashSignRegex.Match(part);
    if (equalMatch.Success)
    {
        var firstPart = equalMatch.Groups["FirstPart"].Value;
        var focalLength = int.Parse(equalMatch.Groups["Digit"].Value);
        var hashValue = (int)CalculateHash(firstPart, 0);

        var existingLens = boxes[hashValue].SingleOrDefault(e => e.FirstPart == firstPart);
        if (existingLens is not null)
        {
            existingLens.FocalLength = focalLength;
        }
        else
        {
            boxes[hashValue].Add(new Box
            {
                FirstPart = firstPart,
                FocalLength = focalLength,
                HashValue = hashValue
            });
        }
        boxes[hashValue] = boxes[hashValue].ToList();
    }
    else if (dashMatch.Success)
    {
        var firstPart = dashMatch.Groups["FirstPart"].Value;
        var hashValue = (int)CalculateHash(firstPart, 0);
        boxes[hashValue] = boxes[hashValue].Where(e => e.FirstPart != firstPart).ToList();
    }
}

for (int i = 0; i < boxes.Count; i++)
{
    for (int j = 0; j < boxes[i].Count; j++)
    {
        part2Result += ((i + 1) * (j + 1) * boxes[i][j].FocalLength);
    }
}

Console.WriteLine($"Part 2 result: {part2Result}");

return;

long CalculateHash(string value, long finalResultAccumulator)
{
    var asciiValues = Encoding.ASCII.GetBytes(value);
    var result = 0;
    for (var i = 0; i < value.Length; i++)
    {
        result += asciiValues[i];
        result *= 17;
        result %= 256;
    }

    return finalResultAccumulator + result;
}

record Box
{
    public string FirstPart { get; set; } = null!;
    public long HashValue { get; set; }
    public long FocalLength { get; set; }
}
