using CsvHelper;
using CsvHelper.Configuration;

namespace Feature.MonitoringObservers.Parser;
internal sealed class MonitoringObserverImportModelMapper : ClassMap<MonitoringObserverImportModel>
{
    public MonitoringObserverImportModelMapper()
    {
        Map(m => m.Email).Name("Email"); // 0
        Map(m => m.FirstName).Name("FirstName"); // 1
        Map(m => m.LastName).Name("LastName"); // 2
        Map(m => m.PhoneNumber).Name("PhoneNumber"); // 3
        Map(m => m.Tags).Convert(ReadTags); // 4..
    }

    private static string[] ReadTags(ConvertFromStringArgs row)
    {
        var tags = new HashSet<string>();

        for (var i = 4; i < row.Row?.ColumnCount; i++)
        {
            var value = row.Row[i];
            tags.Add(value);
        }

        return tags.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
    }
}
