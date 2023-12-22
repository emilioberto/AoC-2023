var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

long part1Result = 0;
long part2Result = 0;

var matrix = Array.Empty<string[]>();
await foreach (var line in File.ReadLinesAsync(filePath))
{
    if (string.IsNullOrEmpty(line))
    {
        var colReflection = CheckColsReflection(matrix);
        var rowReflection = CheckRowsReflection(matrix);

        part1Result += (colReflection ?? 0) + (100 * (rowReflection ?? 0));

        matrix = Array.Empty<string[]>();
        continue;
    }

    matrix = matrix.Append(line.ToCharArray().Select(e => e.ToString()).ToArray()).ToArray();
}

var colReflection1 = CheckColsReflection(matrix);
var rowReflection1 = CheckRowsReflection(matrix);
part1Result += (colReflection1 ?? 0) + (100 * (rowReflection1 ?? 0));

Console.WriteLine($"Part 1 result: {part1Result}");


int? CheckRowsReflection(string[][] matrix)
{
    var rows = matrix.Select((row, index) => (string.Join("", row), index)).ToArray();
    var groupedRows = rows.GroupBy(e => e.Item1);

    var reflection = groupedRows.Where(e => e.Count() == 2 && Math.Abs(e.First().index - e.Last().index) == 1).ToArray().SingleOrDefault();

    if (reflection is null)
    {
        return null;
    }

    var matches = true;
    var left = reflection.First().index;
    var right = reflection.Last().index;
    while (matches)
    {
        if (left < 1 || right > rows.Length - 2)
        {
            matches = false;
            continue;
        }

        if (rows[left - 1].Item1 == rows[right + 1].Item1)
        {
            left--;
            right++;
        }
        else
        {
            left++;
            right--;
            matches = false;
        }
    }

    if (left > 1 || right < rows.Length - 2)
    {
        return null;
    }

    return reflection.First().index + 1;
}

int? CheckColsReflection(string[][] matrix)
{
    var rows = matrix[0].Select((_, index) =>
    {
        var column = "";
        for (int i = 0; i < matrix.Length - 1; i++)
        {
            column += matrix[i][index];
        }
        return (column, index);
    }).ToArray();

    var groupedRows = rows.GroupBy(e => e.Item1);

    var reflection = groupedRows.Where(e => e.Count() == 2 && Math.Abs(e.First().index - e.Last().index) == 1).ToArray().SingleOrDefault();

    if (reflection is null)
    {
        return null;
    }

    var matches = true;
    var left = reflection.First().index;
    var right = reflection.Last().index;
    while (matches)
    {
        if (left < 1 || right > rows.Length - 2)
        {
            matches = false;
            continue;
        }

        if (rows[left - 1].Item1 == rows[right + 1].Item1)
        {
            left--;
            right++;
        }
        else
        {
            left++;
            right--;
            matches = false;
        }
    }

    return reflection.First().index + 1;
}
