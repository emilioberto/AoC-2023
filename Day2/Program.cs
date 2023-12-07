using System.Text.RegularExpressions;

var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");


var gameNumberPattern = @"(?<GameId>(\d+))";
var gamePattern = $@"(?:Game {gameNumberPattern}:\s)";

var amountPattern = @"(?<Amount>(\d+))";
var colorPattern = @"(?<Color>(\w+))";
var cubesPattern = $@"(?<Cubes>({amountPattern}\s{colorPattern},?\s?))";
var setPattern = $@"(?<Set>({cubesPattern}+))";
var setAndDelimiterPattern = $@"(?<SetAndDelimiter>({setPattern};?\s?))";
var setsPattern = $@"(?<Sets>({setAndDelimiterPattern}+))";
var regex = new Regex($@"{gamePattern}{setsPattern}");

var part1Result = 0;
var part2Result = 0;
await foreach (var line in File.ReadLinesAsync(filePath))
{
    var game = GetGame(line);

    var maxRedCubes = game.Sets.SelectMany(e => e.Cubes).Where(e => e.Color == "red").Max(e => e.Amount);
    var maxBlueCubes = game.Sets.SelectMany(e => e.Cubes).Where(e => e.Color == "blue").Max(e => e.Amount);
    var maxGreenCubes = game.Sets.SelectMany(e => e.Cubes).Where(e => e.Color == "green").Max(e => e.Amount);

    if (maxRedCubes <= 12 && maxGreenCubes <= 13 && maxBlueCubes <= 14)
    {
        part1Result += game.Id;
    }

    part2Result += (maxRedCubes * maxBlueCubes * maxGreenCubes);
}

Console.WriteLine($"Part 1 result: {part1Result}");
Console.WriteLine($"Part 2 result: {part2Result}");

return;

Game GetGame(string input)
{
    var matches = regex.Matches(input);
    var groups = matches[0].Groups;
    groups.TryGetValue("GameId", out var gameIds);
    groups.TryGetValue("Cubes", out var cubes);
    groups.TryGetValue("Set", out var sets);
    groups.TryGetValue("Amount", out var amounts);
    groups.TryGetValue("Color", out var colors);

    var gameNumber = int.Parse(gameIds!.Value);
    var gameSets = sets!.Captures!.Select((setCapture, setIndex) =>
    {
        var setCubes = setCapture.Value.Split(", ").Select(stringCubes =>
        {
            var cubesInfo = stringCubes.Split(" ");
            return new Cubes
            {
                Amount = int.Parse(cubesInfo[0]),
                Color = cubesInfo[1]
            };
        }).ToList();

        return new Set()
        {
            Cubes = setCubes
        };
    });

    return new Game
    {
        Id = gameNumber,
        Sets = gameSets.ToList()
    };
}


// Models

record Game
{
    public int Id { get; set; }
    public List<Set> Sets { get; set; } = new();
}

record Set
{
    public List<Cubes> Cubes { get; set; } = new();
}

record Cubes
{
    public int Amount { get; set; }
    public string Color { get; set; } = null!;
}
