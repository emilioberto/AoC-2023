var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

int part1Result = 0;
int part2Result = 0;

var matrix = Array.Empty<string[]>();
int? rowsPart2;
int? rows;
int? columns;
await foreach (var line in File.ReadLinesAsync(filePath))
{
    if (string.IsNullOrEmpty(line))
    {
        rows = CheckResult(matrix);
        columns = CheckResult(GetInvertedMatrix(matrix));
        part1Result += (columns ?? 0) + (100 * (rows ?? 0));

        rowsPart2 = CheckResult(matrix, true);
        part2Result += 100 * rowsPart2!.Value;

        matrix = Array.Empty<string[]>();
        continue;
    }

    matrix = matrix.Append(line.ToCharArray().Select(e => e.ToString()).ToArray()).ToArray();
}

rows = CheckResult(matrix);
columns = CheckResult(GetInvertedMatrix(matrix));
part1Result += (columns ?? 0) + (100 * (rows ?? 0));

rowsPart2 = CheckResult(matrix, true);
part2Result += 100 * rowsPart2!.Value;

Console.WriteLine($"Part 1 result: {part1Result}");
Console.WriteLine($"Part 1 result: {part2Result}");

string[][] GetInvertedMatrix(string[][] matrix)
{
    var invertedMatrix = new List<List<string>>();

    for (int y = 0; y < matrix.Length; y++)
    {
        for (int x = 0; x < matrix[0].Length; x++)
        {
            if (y == 0)
            {
                invertedMatrix.Add(new List<string>());
            }

            invertedMatrix[x].Add(matrix[y][x]);
        }
    }

    return invertedMatrix.Select(row => row.ToArray()).ToArray();
}


int? CheckResult(string[][] matrix, bool isPart2 = false)
{
    var hashMap = new List<long>();
    foreach (var row in matrix)
    {
        var result = 0L;
        for (long i = 0; i < row.Length; i++)
        {
            result += row[i] is "." ? (long)(Math.Pow(2, i)) : 0L;
        }

        hashMap.Add(result);
    }

    // EG: 306 331 252 252 331 307 330
    // Console.WriteLine(string.Join(" ", hashmap));

    if (isPart2)
    {
        matrix = FixMatrix(matrix, hashMap);
        return CheckResult(matrix);
    }

    for (var i = 0; i < hashMap.Count - 1; i++)
    {
        if (hashMap[i] == hashMap[i + 1] && CheckNeighbors(hashMap, i) is not null)
        {
            return i + 1;
        }
    }

    return null;
}

int? CheckNeighbors(List<long> hashMap, int index)
{
    var leftOffset = index;
    var rightOffset = index + 1;
    while (leftOffset >= 0 && rightOffset <= hashMap.Count - 1)
    {
        if (hashMap[leftOffset] == hashMap[rightOffset])
        {
            leftOffset--;
            rightOffset++;
        }
        else
        {
            return null;
        }
    }

    return index;
}

string[][] FixMatrix(string[][] matrix, List<long> hashMap)
{
    var rowsWithoutMatch = hashMap.Where((toFind, i) => hashMap.Count(hash => hash == toFind) == 1).ToArray();

    for (int i = 0; i < hashMap.Count; i++)
    {
        for (int j = 0; j < hashMap.Count; j++)
        {
            var difference = GetRowDifference(matrix, i, j);
            if (difference.Count(e => e != " ") == 1)
            {
                matrix[i] = matrix[j];
                return matrix;
            }
        }
    }

    return matrix;
}

string[] GetRowDifference(string[][] strings, int offsetLeft1, int offsetRight1)
{
    var difference = strings[offsetLeft1].Select<string, string>((character, index) =>
    {
        if (character == strings[offsetRight1].ToArray()[index])
        {
            return " ";
        }

        return character;
    }).ToArray();

    return difference;
}

void PrintMatrix(string[][] strings)
{
    for (var y = 0; y < strings.Length; y++)
    {
        var row = strings[y];
        Console.WriteLine(string.Join("", row.Select(s => s)));
    }
}
