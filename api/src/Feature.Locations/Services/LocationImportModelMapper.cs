using CsvHelper.Configuration;
using Vote.Monitor.Core.Models;

namespace Feature.Locations.Services;
internal sealed class LocationImportModelMapper : ClassMap<LocationImportModel>
{
    public LocationImportModelMapper()
    {
        Map(m => m.Level1).Name("level1"); // 0
        Map(m => m.Level2).Name("level2"); // 1
        Map(m => m.Level3).Name("level3"); // 2
        Map(m => m.Level4).Name("level4"); // 3
        Map(m => m.Level5).Name("level5"); // 4
        Map(m => m.DisplayOrder).Name("displayOrder").Optional(); //5
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
