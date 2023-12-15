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
    { '5', 'L' },
    { '4', 'M' },
    { '3', 'N' },
    { '2', 'O' },
};

var summaryList = new List<HandSummary>();
await foreach (var line in File.ReadLinesAsync(filePath))
{
    var match = handsBidsRegex.Match(line);
    var hand = match.Groups["Hand"].Value;
    var bid = long.Parse(match.Groups["Bid"].Value);

    var handValue = CalculateHandValue(hand.ToCharArray().Select(e => e.ToString()).GroupBy(e => e));

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
            OriginalHand = hand,
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

orderedSummary = summaryList.Select(handSummary =>
{
    if (!handSummary.OriginalHand.Contains("J"))
    {
        return handSummary;
    }

    var jokers = handSummary.OriginalHand.Where(e => e == 'J').ToArray();
    handSummary.HandValue = CalculateHandValue(handSummary.OriginalHand.ToCharArray().Select(e => e.ToString()).Where(e => e is not "J").GroupBy(e => e));
    cardsValue.TryGetValue('J', out var oldJokerMappedValue);
    foreach (var joker in jokers)
    {
        handSummary.HandValue = handSummary.HandValue switch
        {
            HandValue.None => HandValue.HighCard,
            HandValue.HighCard => HandValue.OnePair,
            HandValue.OnePair => HandValue.ThreeOfAKind,
            HandValue.TwoPair => HandValue.FullHouse,
            HandValue.ThreeOfAKind => HandValue.FourOfAKind,
            HandValue.FourOfAKind => HandValue.FiveOfAKind,
            HandValue.FullHouse => HandValue.FullHouse,
            HandValue.FiveOfAKind => HandValue.FiveOfAKind,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    handSummary.Hand = handSummary.Hand.Replace(oldJokerMappedValue.ToString(), "Z");
    return handSummary;
}).ToArray();


orderedSummary = orderedSummary
    .OrderBy(e => e.HandValue)
    .GroupBy(e => e.HandValue)
    .Select(e => e.OrderByDescending(handSummary => handSummary.Hand))
    .SelectMany(e => e.ToArray())
    .ToArray();

for (int i = 1; i <= orderedSummary.Length; i++)
{
    part2Result += (orderedSummary[i - 1].Bid * (i));
}

Console.WriteLine($"Part 2 result: {part2Result}");

internal record HandSummary
{
    public string Hand { get; set; } = null!;
    public string OriginalHand { get; set; } = null!;
    public long Bid { get; set; }
    public HandValue HandValue { get; set; }
}

internal enum HandValue
{
    None = 0,
    HighCard = 1,
    OnePair = 2,
    TwoPair = 3,
    ThreeOfAKind = 4,
    FullHouse = 5,
    FourOfAKind = 6,
    FiveOfAKind = 7
}

internal partial class Program
{
    static HandValue CalculateHandValue(IEnumerable<IGrouping<string, string>> hand)
    {
        hand = hand.ToArray();

        if (!hand.Any())
        {
            return HandValue.None;
        }

        if (hand.Any(e => e.Count() == 5))
        {
            return HandValue.FiveOfAKind;
        }

        if (hand.Any(e => e.Count() == 4))
        {
            return HandValue.FourOfAKind;
        }

        if (hand.Any(e => e.Count() == 3) && hand.Any(e => e.Count() == 2))
        {
            return HandValue.FullHouse;
        }

        if (hand.Any(e => e.Count() == 3) && !hand.Any(e => e.Count() == 2))
        {
            return HandValue.ThreeOfAKind;
        }

        if (hand.Count(e => e.Count() == 2) == 2)
        {
            return HandValue.TwoPair;
        }

        if (hand.Count(e => e.Count() == 2) == 1)
        {
            return HandValue.OnePair;
        }

        return HandValue.HighCard;
    }
}
