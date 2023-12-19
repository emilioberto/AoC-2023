var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");

long part1Result = 0;
long part2Result = 0;

var matrix = new List<List<string>>();
await foreach (var line in File.ReadLinesAsync(filePath))
{
    var row = line.ToCharArray().Select(e => e.ToString()).ToList();
    matrix.Add(row);
}

var invertedMatrix = new List<List<string>>();
for (var x = 0; x < matrix[0].Count; x++)
{
    var invertedRow = new List<string>();
    foreach (var row in matrix)
    {
        invertedRow.Add(row[x]);
    }

    invertedMatrix.Add(invertedRow);
}


var stars = new List<(long y, long x, long number)>();
for (var y = 0; y < matrix.Count; y++)
{
    for (var x = 0; x < matrix[0].Count; x++)
    {
        if (matrix[y][x] is "#")
        {
            stars.Add((y, x, stars.Count + 1));
        }
    }
}

int cosmicExpansionMultiplier = 2;

var distances = CaculateDistances(stars, matrix, invertedMatrix, cosmicExpansionMultiplier);

part1Result = distances.Sum(e => e.distance);
Console.WriteLine($"Part 1 result: {part1Result}");

cosmicExpansionMultiplier = 1000000;
distances = CaculateDistances(stars, matrix, invertedMatrix, cosmicExpansionMultiplier);
part2Result = distances.Sum(e => e.distance);
Console.WriteLine($"Part 1 result: {part2Result}");

List<(long starA, long starB, long distance)> CaculateDistances(List<(long y, long x, long number)> valueTuples, List<List<string>> list, List<List<string>> invertedMatrix1, long i)
{
    var distances1 = new List<(long starA, long starB, long distance)>();
    for (var current = 0; current < valueTuples.Count; current++)
    {
        var currentStar = valueTuples[current];
        for (var next = current + 1; next < valueTuples.Count; next++)
        {
            var nextStar = valueTuples[next];
            var x1 = Math.Max(currentStar.x, nextStar.x);
            var x2 = Math.Min(currentStar.x, nextStar.x);
            var y1 = Math.Max(currentStar.y, nextStar.y);
            var y2 = Math.Min(currentStar.y, nextStar.y);

            var expandedRows = list.Where((row, y) => y > y2 && y < y1 && row.All(e => e is ".")).Count();
            var expandedColumns = invertedMatrix1.Where((column, x) => x > x2 && x < x1 && column.All(e => e is ".")).Count();
            var xDistance = (x1 == x2 ? 0 : x1 - x2);
            var yDistance = (y1 == y2 ? 0 : y1 - y2);
            var distance = xDistance + yDistance  + (expandedColumns * i - expandedColumns) + (expandedRows * i - expandedRows);

            distances1.Add((currentStar.number, nextStar.number, distance));
        }
    }

    return distances1;
}
