using CsvHelper.Configuration;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.Feature.PollingStation.Services;
internal sealed class PollingStationImportModelMapper : ClassMap<PollingStationImportModel>
{
    public PollingStationImportModelMapper()
    {
        Map(m => m.Level1).Name("Level1"); // 0
        Map(m => m.Level2).Name("Level2"); // 1
        Map(m => m.Level3).Name("Level3"); // 2
        Map(m => m.Level4).Name("Level4"); // 3
        Map(m => m.Level5).Name("Level5"); // 4
        Map(m => m.Number).Name("Number"); // 5

        Map(m => m.Address).Name("Address"); // 6
        Map(m => m.DisplayOrder).Name("DisplayOrder"); //7
        Map(m => m.Tags).Convert(ReadTags); // 8 -> end
    }

    private static List<TagImportModel> ReadTags(ConvertFromStringArgs row)
    {
        var tags = new List<TagImportModel>();

        for (var i = 8; i < row.Row?.HeaderRecord?.Length; i++)
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
