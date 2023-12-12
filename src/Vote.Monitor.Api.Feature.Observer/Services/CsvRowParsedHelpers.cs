using System.Text;
namespace Vote.Monitor.Api.Feature.Observer.Services;

public static class CsvRowParsedHelpers<T> where T : class
{

    public static string ConstructErrorFileContent(IEnumerable<CsvRowParsed<T>> items, char sepator = ',')
    {
        var sb = new StringBuilder();
        foreach (var item in items)
        {
            sb.Append(item.OriginalRow.ReplaceLineEndings(""));
            if (!item.IsSuccess)
            {
                sb.Append(sepator).Append(item.ErrorMessage);

            }
            sb.Append('\n');
        }
        return sb.ToString();
    }
}
