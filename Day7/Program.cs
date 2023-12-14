using System.Text.RegularExpressions;

var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

long part1Result = 0;
long part2Result = 0;

var handsBidsRegex = new Regex(@"(?<Hand>([23456789AKQJT]{5}))\s+(?<Bid>(\d*))", RegexOptions.Multiline);
var cardsValue = new Dictionary<char, char>
{
    { 'A', 'A' },
    { 'K', 'B' },
    { 'Q', 'C' },
    { 'J', 'D' },
    { 'T', 'E' },
    { '9', 'F' },
    { '8', 'G' },
    { '7', 'H' },
    { '6', 'I' },
    { '5', 'J' },
    { '4', 'K' },
    { '3', 'L' },
    { '2', 'M' },
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

    var handWithReplacedCards = hand!;
    foreach (var cardKeyValue in cardsValue)
    {
        handWithReplacedCards = handWithReplacedCards.Replace(cardKeyValue.Key, cardKeyValue.Value);
    }

    summaryList.Add(
        new()
        {
            Bid = bid,
            Hand = handWithReplacedCards,
            HandValue = handValue
        }
    );
}

var orderedSummary = summaryList
    .OrderBy(e => e.HandValue)
    .GroupBy(e => e.HandValue)
    .Select(e => e.ToArray().OrderByDescending(handSummary => handSummary.Hand))
    .SelectMany(e => e.ToArray())
    .ToArray();

for (int i = 1; i <= orderedSummary.Length; i++)
{
    part1Result += (orderedSummary[i - 1].Bid * (i));
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
