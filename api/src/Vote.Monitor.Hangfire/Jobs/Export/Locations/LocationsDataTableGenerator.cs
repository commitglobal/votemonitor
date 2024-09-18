using System.Text.Json;
using Vote.Monitor.Hangfire.Jobs.Export.Locations.ReadModels;

namespace Vote.Monitor.Hangfire.Jobs.Export.Locations;

public class LocationsDataTableGenerator
{
    private readonly List<string> _header;
    private readonly List<List<object>> _dataTable;
    private readonly List<LocationData> _locations = [];

    private LocationsDataTableGenerator(List<string> header, List<List<object>> dataTable)
    {
        _header = header;
        _dataTable = dataTable;
    }

    internal static LocationsDataTableGenerator For(List<string> header, List<List<object>> dataTable)
    {
        return new LocationsDataTableGenerator(header, dataTable);
    }

    public LocationsDataTableGenerator For(params LocationModel[] locations)
    {
        foreach (var location in locations)
        {
            MapLocation(location);

            var row = new List<object>
            {
                location.Id.ToString(),
                location.Level1,
                location.Level2,
                location.Level3,
                location.Level4,
                location.Level5,
                location.DisplayOrder
            };

            _dataTable.Add(row);
        }

        return this;
    }

    private void MapLocation(LocationModel location)
    {
        LocationData locationData = LocationData.For(location);

        _locations.Add(locationData);
    }

    public (List<string> header, List<List<object>> dataTable) Please()
    {
        // get the tags
        var availableTags = _locations
            .SelectMany(x => x.Tags.Keys)
            .ToHashSet()
            .Order()
            .ToList();

        _header.AddRange(availableTags);

        for (var i = 0; i < _locations.Count; i++)
        {
            var location = _locations[i];
            var row = _dataTable[i];

            var tags = WriteTags(location.Tags, availableTags);

            row.AddRange(tags);
        }

        return (_header, _dataTable);
    }

    private static List<string> WriteTags(Dictionary<string, string> tags, List<string> availableTags)
    {
        return availableTags.Select(tag => tags.TryGetValue(tag, out var value) ? value : string.Empty).ToList();
    }

    internal class LocationData
    {
        public Guid Id { get; }
        public readonly Dictionary<string, string> Tags = new();

        private LocationData(LocationModel location)
        {
            Id = location.Id;
        }

        public static LocationData For(LocationModel location)
        {
            var locationData = new LocationData(location);

            locationData.WithTags(location.Tags);

            return locationData;
        }

        private void WithTags(JsonDocument tags)
        {
            foreach (JsonProperty property in tags.RootElement.EnumerateObject())
            {
                Tags.Add(property.Name, property.Value.GetString() ?? "");
            }
        }
    }
}