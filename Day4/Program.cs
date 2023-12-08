using System.Text;
using System.Text.RegularExpressions;

var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

long part1Result = 0;
long part2Result = 0;

var cards = new List<Card>();

var cardRegex = new Regex(@"Card\s*(?<CardNumber>(\d+)):\s+(?<WinningNumbers>(((?<WinningNumber>(\d+))\s+))+)\|\s+(?<ElfNumbers>(((?<ElfNumber>(\d+))\s*))+)");
await foreach (var line in File.ReadLinesAsync(filePath))
{
    var match = cardRegex.Match(line);
    var cardNumber = int.Parse(match.Groups["CardNumber"].Value);
    var winningNumbers = match.Groups["WinningNumber"].Captures
        .Select(e => double.Parse(e.Value))
        .ToArray();
    var elfNumbers = match.Groups["ElfNumber"].Captures
        .Select(e => double.Parse(e.Value))
        .ToArray();
    var elfWinningNumbers = elfNumbers
        .Where(e => winningNumbers.Contains(e))
        .ToArray();

    cards.Add(new()
    {
        Number = cardNumber,
        WinningNumbers = winningNumbers,
        ElfNumbers = elfNumbers,
        ElfWinningNumbers = elfWinningNumbers,
    });
}

foreach (var card in cards)
{
    var cardResult = 0;
    foreach (var _ in card.ElfWinningNumbers)
    {
        cardResult = cardResult == 0
            ? 1
            : cardResult * 2;
    }
    part1Result += cardResult;
}

Console.WriteLine($"Part 1 result: {part1Result}");


// Part 2 is slow af. Do we care? Not really :D
foreach (var currentCard in cards)
{
    for (var i = 0; i < currentCard.Instances; i++)
    {
        var freeCopiesCount = currentCard.ElfWinningNumbers.Length;
        foreach (var nextCard in cards.Where(e => e.Number > currentCard.Number && e.Number <= currentCard.Number + freeCopiesCount))
        {
            nextCard.Instances++;
        }
    }
}

part2Result = cards.Sum(e => e.Instances);

Console.WriteLine($"Part 2 result: {part2Result}");


class Card
{
    public int Number { get; set; }
    public double[] WinningNumbers { get; set; }
    public double[] ElfNumbers { get; set; }
    public double[] ElfWinningNumbers { get; set; }
    public int Instances { get; set; } = 1;
}
