using CsvHelper;
using CsvHelper.Configuration;

namespace Vote.Monitor.Feature.PollingStation.ImportPollingStations;
internal sealed class PollingStationImportDTOMap : ClassMap<PollingStationImport>
{
    public PollingStationImportDTOMap()
    {
        Map(m => m.DisplayOrder).Name("DisplayOrder");
        Map(m => m.Address).Name("Address");
        Map(m => m.Tags).Convert(args => ReadAdditionalColumns(args));
    }

    private Dictionary<string, string> ReadAdditionalColumns(ConvertFromStringArgs row)
    {
        var additionalColumns = new Dictionary<string, string>();

        for (var i = 2; i < row.Row?.HeaderRecord?.Length; i++)
        {
            additionalColumns[row.Row.HeaderRecord[i]] = row.Row[i];
        }
        return additionalColumns;
    }
}
