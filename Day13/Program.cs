var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

long part1Result = 0;
long part2Result = 0;

var matrix = Array.Empty<string[]>();
await foreach (var line in File.ReadLinesAsync(filePath))
{
    if (string.IsNullOrEmpty(line))
    {
        var rows = CheckResult(matrix);
        var columns = CheckResult(GetInvertedMatrix(matrix));
        part1Result += (columns ?? 0) + (100 * (rows ?? 0));

        // var colReflection = CheckRowsReflection(matrix);
        // var rowReflection = CheckRowsReflection(GetInvertedMatrix(matrix));
        // part1Result += (rowReflection ?? 0) + (100 * (colReflection ?? 0));

        matrix = Array.Empty<string[]>();
        continue;
    }

    matrix = matrix.Append(line.ToCharArray().Select(e => e.ToString()).ToArray()).ToArray();
}

var rows2 = CheckResult(matrix);
var columns2 = CheckResult(GetInvertedMatrix(matrix));
part1Result += (columns2 ?? 0) + (100 * (rows2 ?? 0));

Console.WriteLine($"Part 1 result: {part1Result}");

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


int? CheckResult(string[][] matrix)
{
    var hashmap = new List<long>();
    foreach (var row in matrix)
    {
        var result = 0L;
        for (long i = 0; i < row.Length; i++)
        {
            result += row[i] is "." ? (long)(Math.Pow(2, i)) : 0L;
        }

        hashmap.Add(result);
    }

    // EG: 306 331 252 252 331 307 330
    Console.WriteLine(string.Join(" ", hashmap));

    for (var i = 0; i < hashmap.Count - 1; i++)
    {
        if (hashmap[i] == hashmap[i + 1] && CheckNeighbors(hashmap, i))
        {
            return i + 1;
        }
    }

    return null;
}

bool CheckNeighbors(List<long> hashMap, int index)
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
            return false;
        }
    }

    return true;
}
