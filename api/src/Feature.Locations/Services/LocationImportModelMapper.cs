using CsvHelper.Configuration;
using Vote.Monitor.Core.Models;

namespace Feature.Locations.Services;
internal sealed class LocationImportModelMapper : ClassMap<LocationImportModel>
{
    public LocationImportModelMapper()
    {
        Map(m => m.Level1).Name("Level1"); // 0
        Map(m => m.Level2).Name("Level2"); // 1
        Map(m => m.Level3).Name("Level3"); // 2
        Map(m => m.Level4).Name("Level4"); // 3
        Map(m => m.Level5).Name("Level5"); // 4
        Map(m => m.DisplayOrder).Name("DisplayOrder"); //5
        Map(m => m.Tags).Convert(ReadTags); // 6 -> end
    }

    private static List<TagImportModel> ReadTags(ConvertFromStringArgs row)
    {
        var tags = new List<TagImportModel>();

        for (var i = 6; i < row.Row?.HeaderRecord?.Length; i++)
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
