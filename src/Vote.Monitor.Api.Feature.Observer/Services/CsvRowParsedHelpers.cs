using System.Text;
namespace Vote.Monitor.Api.Feature.Observer.Services;

public static class CsvRowParsedHelpers
{
    public static string ConstructErrorFileContent<T>(this IEnumerable<CsvRowParsed<T>> items, char separator = ',') where T : class
    {
        var sb = new StringBuilder();
        foreach (var item in items)
        {
            sb.Append(item.OriginalRow.ReplaceLineEndings(""));
            if (!item.IsSuccess)
            {
                sb.Append(separator).Append(item.ErrorMessage);
            }

            sb.Append(Environment.NewLine);
        }

        return sb.ToString();
    }


    public static bool CheckAndSetDuplicatesLines<T>(this List<CsvRowParsed<T>> rows) where T : class
    {
        Dictionary<int, int> set = new();
        bool containsDuplicates = false;
        for (int i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            if (row.IsSuccess && row.Value == null) continue;
            if (set.ContainsKey(row.Value!.GetHashCode()))
            {
                int firstRowIndex = set[row.Value.GetHashCode()];
                containsDuplicates = true;
                row.IsSuccess = false;
                row.ErrorMessage += $"Duplicated data found. First row where you can find the duplicate data is {firstRowIndex + 1}";
                rows[firstRowIndex].IsSuccess = false;
                rows[firstRowIndex].ErrorMessage = string.IsNullOrEmpty(rows[firstRowIndex].ErrorMessage) ? $"Duplicated data found. Row(s) where you can find the duplicate data are {i + 1}" : rows[firstRowIndex].ErrorMessage + $", {i + 1}";
            }
            else
            {
                set.Add(row.Value!.GetHashCode(), i);
            }
        }

        return containsDuplicates;
    }

}
