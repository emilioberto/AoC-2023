using System.Text.RegularExpressions;

var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

long part1Result = 0;
long part2Result = 0;

var handsBidsRegex = new Regex(@"(?<Hand>([23456789AKQJT]{5}))\s+(?<Bid>(\d*))", RegexOptions.Multiline);
var cardsValue = new Dictionary<string, int>
{
    { "A", 14 },
    { "K", 13 },
    { "Q", 12 },
    { "J", 11 },
    { "T", 10 },
    { "9", 9 },
    { "8", 8 },
    { "7", 7 },
    { "6", 6 },
    { "5", 5 },
    { "4", 4 },
    { "3", 3 },
    { "2", 2 },
};

var summaryList = new List<HandSummary>();
await foreach (var line in File.ReadLinesAsync(filePath))
{
    var match = handsBidsRegex.Match(line);
    var hand = match.Groups["Hand"].Value;
    var bid = long.Parse(match.Groups["Bid"].Value);

    var groupedCards = hand.ToCharArray().Select(e => e.ToString()).GroupBy(e => e);
    var handValue = HandValue.HighCard;

    if (groupedCards.Count() == 1)
    {
        handValue = HandValue.FiveOfAKind;
    }
    else if (groupedCards.Count() == 2)
    {
        if (groupedCards.Any(e => e.Count() == 4))
        {
            handValue = HandValue.FourOfAKind;
        }

        if (groupedCards.Any(e => e.Count() == 3))
        {
            handValue = HandValue.FullHouse;
        }
    }
    else if (groupedCards.Count() == 3 && groupedCards.Any(e => e.Count() == 3))
    {
        handValue = HandValue.ThreeOfAKind;
    }
    else if (groupedCards.Count() == 3 && groupedCards.Where(e => e.Count() == 2).Count() == 2)
    {
        handValue = HandValue.TwoPair;
    }
    else if (groupedCards.Count() == 4)
    {
        handValue = HandValue.OnePair;
    }
    else if (groupedCards.Count() == 5)
    {
        handValue = HandValue.HighCard;
    }

    summaryList.Add(
        new()
        {
            Bid = bid,
            Hand = hand,
            HandValue = handValue
        }
    );
}

var groupedAndOrderedSummaryList = summaryList
    .GroupBy(e => e.HandValue)
    .OrderBy(e => e.Key);

foreach (var group in groupedAndOrderedSummaryList)
{
    if (group.Count() > 1)
    {
    }
}


Console.WriteLine($"Part 1 result: {part1Result}");

Console.WriteLine($"Part 2 result: {part2Result}");

internal record HandSummary
{
    public string Hand { get; set; } = null!;
    public long Bid { get; set; }
    public HandValue HandValue { get; set; }
}

internal enum HandValue
{
    HighCard = 1,
    OnePair = 2,
    TwoPair = 3,
    ThreeOfAKind = 4,
    FullHouse = 5,
    FourOfAKind = 6,
    FiveOfAKind = 7
}
