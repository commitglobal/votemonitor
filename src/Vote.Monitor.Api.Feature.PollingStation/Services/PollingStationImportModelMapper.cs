using CsvHelper.Configuration;

namespace Vote.Monitor.Api.Feature.PollingStation.Services;
internal sealed class PollingStationImportModelMapper : ClassMap<PollingStationImportModel>
{
    public PollingStationImportModelMapper()
    {
        Map(m => m.DisplayOrder).Name("DisplayOrder");
        Map(m => m.Address).Name("Address");
        Map(m => m.Tags).Convert(ReadTags);
    }

    private static List<TagImportModel> ReadTags(ConvertFromStringArgs row)
    {
        var tags = new List<TagImportModel>();

        for (var i = 2; i < row.Row?.HeaderRecord?.Length; i++)
        {
            var name = row.Row.HeaderRecord[i];
            var value = row.Row[i];
            tags.Add(new()
            {
                Name = name,
                Value = value
            });
        }

        return tags;
    }
}
