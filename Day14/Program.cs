using System.Text;

var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

long part1Result = 0;
long part2Result = 0;

var matrix = new List<List<string>>();
await foreach (var line in File.ReadLinesAsync(filePath))
{
    var row = line.ToCharArray().Select(e => e.ToString()).ToList();
    matrix.Add(row);
}

var rockMoved = true;
var part1Matrix = CopyMatrix(matrix);
part1Matrix = TiltNorth(part1Matrix);

part1Result = CalculatePart1Result(part1Matrix);
Console.WriteLine($"Part 1 result is: {part1Result}");

var cycles = 1000000000L;

var part2Matrix = CopyMatrix(matrix);
part2Matrix = Cycle(part2Matrix, cycles);

part2Result = CalculatePart1Result(part2Matrix);
Console.WriteLine($"Part 1 result is: {part2Result}");

return 0;


static void PrintMatrix(List<List<string>> strings)
{
    foreach (var row in strings)
    {
        Console.WriteLine(string.Join("", row.Select(s => s)));
    }
}

static List<List<string>> TiltNorth(List<List<string>> matrix)
{
    var rockMoved = true;
    while (rockMoved)
    {
        rockMoved = false;
        for (int y = 1; y < matrix.Count; y++)
        {
            for (int x = 0; x < matrix[0].Count; x++)
            {
                if (matrix[y][x] is not "O")
                {
                    continue;
                }

                if (matrix[y - 1][x] != ".")
                {
                    continue;
                }

                matrix[y - 1][x] = matrix[y][x];
                matrix[y][x] = ".";
                rockMoved = true;
            }
        }
    }

    return matrix;
}

static List<List<string>> TiltSouth(List<List<string>> matrix)
{
    var rockMoved = true;
    while (rockMoved)
    {
        rockMoved = false;
        for (int y = matrix.Count - 2; y >= 0; y--)
        {
            for (int x = 0; x < matrix[0].Count; x++)
            {
                if (matrix[y][x] is not "O")
                {
                    continue;
                }

                if (matrix[y + 1][x] != ".")
                {
                    continue;
                }

                matrix[y + 1][x] = matrix[y][x];
                matrix[y][x] = ".";
                rockMoved = true;
            }
        }
    }

    return matrix;
}

static List<List<string>> TiltWest(List<List<string>> matrix)
{
    var rockMoved = true;
    while (rockMoved)
    {
        rockMoved = false;
        for (int y = 0; y < matrix.Count; y++)
        {
            for (int x = 1; x < matrix[0].Count; x++)
            {
                if (matrix[y][x] is not "O")
                {
                    continue;
                }

                if (matrix[y][x - 1] != ".")
                {
                    continue;
                }

                matrix[y][x - 1] = matrix[y][x];
                matrix[y][x] = ".";
                rockMoved = true;
            }
        }
    }

    return matrix;
}

static List<List<string>> TiltEast(List<List<string>> matrix)
{
    var rockMoved = true;
    while (rockMoved)
    {
        rockMoved = false;
        for (int y = 0; y < matrix.Count; y++)
        {
            for (int x = matrix[0].Count - 2; x >= 0; x--)
            {
                if (matrix[y][x] is not "O")
                {
                    continue;
                }

                if (matrix[y][x + 1] != ".")
                {
                    continue;
                }

                matrix[y][x + 1] = matrix[y][x];
                matrix[y][x] = ".";
                rockMoved = true;
            }
        }
    }

    return matrix;
}

static List<List<string>> Cycle(List<List<string>> matrix, long cycles)
{
    var hashMap = new Dictionary<string, List<long>>();
    var flag = false;
    for (long i = 1; i <= cycles; i++)
    {
        matrix = TiltNorth(matrix);
        matrix = TiltWest(matrix);
        matrix = TiltSouth(matrix);
        matrix = TiltEast(matrix);

        var hash = CalculateHash(matrix);
        var existingHash = hashMap.TryGetValue(hash, out var value);
        var newValue = new List<long> { i };
        if (existingHash)
        {
            newValue = value!.Select(e => e).Append(i).ToList();
            hashMap.Remove(hash);
        }

        hashMap.Add(hash, newValue);

        if (!flag && newValue.Count > 1)
        {
            var modulo = Math.Abs(newValue.First() - newValue.Last());
            Console.WriteLine($"Fock, i found it repeats every {modulo} so...");
            i = cycles - ((cycles - i) % modulo);
            Console.WriteLine($"... i is now: {i}");
            flag = true;
        }

        string CalculateHash(List<List<string>> matrix)
        {
            var stringifiedMatrix = string.Join("", matrix.Select(row => string.Join("", row.Select(e => e))));
            var textBytes = Encoding.UTF8.GetBytes(stringifiedMatrix);
            return Convert.ToBase64String(textBytes);
        }
    }

    Console.WriteLine(hashMap);

    var gigi = hashMap.Where(e => e.Value.Count > 1 && e.Value.Any(index => cycles % index == 0));

    return matrix;
}

long CalculatePart1Result(List<List<string>> matrix)
{
    var result = 0L;
    var load = matrix.Count;
    for (int y = 0; y < matrix.Count; y++)
    {
        for (int x = 0; x < matrix[0].Count; x++)
        {
            if (matrix[y][x] is "O")
            {
                result += (load - y);
            }
        }
    }

    return result;
}

List<List<string>> CopyMatrix(List<List<string>> matrix1)
{
    return matrix1.Select(list => list.Select(e => e).ToList()).ToList();
}
