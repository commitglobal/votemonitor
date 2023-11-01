using CsvHelper;
using CsvHelper.Configuration;

namespace Vote.Monitor.Feature.PollingStation.Import;
internal sealed class ImportModelMapper : ClassMap<ImportModel>
{
    public ImportModelMapper()
    {
        Map(m => m.DisplayOrder).Name("DisplayOrder");
        Map(m => m.Address).Name("Address");
        Map(m => m.Tags).Convert(ReadAdditionalColumns);
    }

    private static Dictionary<string, string> ReadAdditionalColumns(ConvertFromStringArgs row)
    {
        var additionalColumns = new Dictionary<string, string>();

        for (var i = 2; i < row.Row?.HeaderRecord?.Length; i++)
        {
            var key = row.Row.HeaderRecord[i];
            var value = row.Row[i];
            if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
            {
                additionalColumns[key] = value;
            }
        }

        return additionalColumns;
    }
}
