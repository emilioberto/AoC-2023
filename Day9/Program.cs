using System.Text.RegularExpressions;

var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

var historyRegex = new Regex(@"\d+");

await foreach (var line in File.ReadLinesAsync(filePath))
{
    var matches = historyRegex.Matches(line);
    var numbers = matches.Select(e => e.Value).Select(long.Parse).ToArray();

    var differences = GetDifferencesArray(numbers);

    var result = CalculateResult(numbers);

    long CalculateResult(long[] longs)
    {

    }

    var stop = false;
    while (!stop)
    {
        Console.WriteLine($"Doing shit {string.Join(" ", differences)}");
        differences = GetDifferencesArray(differences);
        stop = differences.Distinct().Count() == 1;
    }


    Console.WriteLine("DONE");
}

long[] GetDifferencesArray(long[] input)
{
    var result = Array.Empty<long>();
    for (int i = 0; i < input.Length - 1; i++)
    {
        if (input[i] == input.Length - 1)
        {
            continue;
        }

        result = result.Append(input[i + 1] - input[i]).ToArray();
    }

    return result;
}
