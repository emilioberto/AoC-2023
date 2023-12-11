using System.Text.RegularExpressions;

var instructionsRegex = new Regex($@"(?<Instructions>([RL]*))[\r\n]*(?<Map>(?<Node>([\d\w]{{3}}))\s=\s\((?<Left>([\d\w]{{3}})),\s(?<Right>([\d\w]{{3}}))\)[\r\n]*)*");
var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
var part1Result = 0m;
var part2Result = 0m;

var input = await File.ReadAllTextAsync(filePath);

var match = instructionsRegex.Match(input);

var instructions = match.Groups["Instructions"].Value.ToCharArray().Select(e => e.ToString()).ToArray();

var map = match.Groups["Map"].Captures;
var nodes = match.Groups["Node"].Captures.Select(e => e.Value).ToArray();
var lefts = match.Groups["Left"].Captures.Select(e => e.Value).ToArray();
var rights = match.Groups["Right"].Captures.Select(e => e.Value).ToArray();

var nodeMap = new List<MapNode>();
for (var i = 0; i < nodes.Length; i++)
{
    nodeMap.Add(new MapNode
    {
        Name = nodes[i],
        Left = lefts[i],
        Right = rights[i]
    });
}

// var currentNode = nodeMap.First(e => e.Name == "AAA");
// while (currentNode.Name is not "ZZZ")
// {
//     foreach (var instruction in instructions)
//     {
//         part1Result++;
//         currentNode = instruction is "L"
//             ? nodeMap.First(e => e.Name == currentNode.Left)
//             : nodeMap.First(e => e.Name == currentNode.Right);
//     }
//
//     if (currentNode.Name is "ZZZ")
//     {
//         break;
//     }
// }


Console.WriteLine($"Part 1: {part1Result}");

//
// var startingNodes = nodeMap.Where(e => e.Name.EndsWith("A")).ToArray();
// while (startingNodes.Any(node => !node.Name.EndsWith("Z")))
// {
//     foreach (var instruction in instructions)
//     {
//         part2Result++;
//
//         for (int i = 0; i < startingNodes.Length; i++)
//         {
//             var nthNode = startingNodes[i];
//
//             startingNodes[i] = instruction is "L"
//                 ? nodeMap.First(e => e.Name == nthNode.Left)
//                 : nodeMap.First(e => e.Name == nthNode.Right);
//         }
//
//         if (startingNodes.All(node => node.Name.EndsWith("Z")))
//         {
//             break;
//         }
//     }
// }


var startingNodes = nodeMap.Where(e => e.Name.EndsWith("A")).ToArray();
var results = new List<long>();
for (int i = 0; i < startingNodes.Length; i++)
{
    var nodeResult = 0;
    while (!startingNodes[i].Name.EndsWith("Z"))
    {
        foreach (var instruction in instructions)
        {
            nodeResult++;
            startingNodes[i] = instruction is "L"
                ? nodeMap.First(e => e.Name == startingNodes[i].Left)
                : nodeMap.First(e => e.Name == startingNodes[i].Right);
        }

        if (startingNodes[i].Name.EndsWith("Z"))
        {
            results.Add(nodeResult);
            break;
        }
    }
}

Console.WriteLine(string.Join(", ", results));

Console.WriteLine(CalculateLCM(results.ToArray()));

// Provare cercando il minimo divisore delle iterazioni che portano a Z per ogni nodo

Console.WriteLine($"Part 2: {part2Result}");

record MapNode
{
    public string Name { get; set; }
    public string Left { get; set; }
    public string Right { get; set; }
}

// Function to calculate GCD (Greatest Common Divisor)

public partial class Program
{
    static long GCD(long a, long b)
    {
        while (b != 0)
        {
            long temp = b;
            b = a % b;
            a = temp;
        }

        return a;
    }

// Function to calculate LCM (Least Common Multiple)
    static long LCM(long a, long b)
    {
        return (a / GCD(a, b)) * b;
    }

// Function to calculate LCM of an array of numbers
    static long CalculateLCM(long[] numbers)
    {
        long lcm = 1;

        foreach (int number in numbers)
        {
            lcm = LCM(lcm, number);
        }

        return lcm;
    }
}
