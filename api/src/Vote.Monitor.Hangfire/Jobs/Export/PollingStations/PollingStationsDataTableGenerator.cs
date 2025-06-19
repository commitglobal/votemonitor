using System.Text.Json;
using Vote.Monitor.Hangfire.Jobs.Export.PollingStations.ReadModels;

namespace Vote.Monitor.Hangfire.Jobs.Export.PollingStations;

public class PollingStationsDataTableGenerator
{
    private readonly List<string> _header;
    private readonly List<List<object>> _dataTable;
    private readonly List<PollingStationData> _pollingStations = [];

    private PollingStationsDataTableGenerator(List<string> header, List<List<object>> dataTable)
    {
        _header = header;
        _dataTable = dataTable;
    }

    internal static PollingStationsDataTableGenerator For(List<string> header, List<List<object>> dataTable)
    {
        return new PollingStationsDataTableGenerator(header, dataTable);
    }

    public PollingStationsDataTableGenerator For(PollingStationModel pollingStation)
    {
        MapPollingStation(pollingStation);

        var row = new List<object>
        {
            pollingStation.Id.ToString(),
            pollingStation.Level1,
            pollingStation.Level2,
            pollingStation.Level3,
            pollingStation.Level4,
            pollingStation.Level5,
            pollingStation.Number,
            pollingStation.Address,
            pollingStation.Latitude?.ToString() ?? "",
            pollingStation.Longitude?.ToString() ?? "",
            pollingStation.DisplayOrder
        };

        _dataTable.Add(row);
        return this;
    }

    public PollingStationsDataTableGenerator ForPollingStations(List<PollingStationModel> pollingStations)
    {
        foreach (var pollingStation in pollingStations)
        {
            For(pollingStation);
        }

        return this;
    }

    private void MapPollingStation(PollingStationModel pollingStation)
    {
        PollingStationData pollingStationData = PollingStationData.For(pollingStation);

        _pollingStations.Add(pollingStationData);
    }

    public (List<string> header, List<List<object>> dataTable) Please()
    {
        // get the tags
        var availableTags = _pollingStations
            .SelectMany(x => x.Tags.Keys)
            .ToHashSet()
            .Order()
            .ToList();

        _header.AddRange(availableTags);

        for (var i = 0; i < _pollingStations.Count; i++)
        {
            var pollingStation = _pollingStations[i];
            var row = _dataTable[i];

            var tags = WriteTags(pollingStation.Tags, availableTags);

            row.AddRange(tags);
        }

        return (_header, _dataTable);
    }

    private static List<string> WriteTags(Dictionary<string, string> tags, List<string> availableTags)
    {
        return availableTags.Select(tag => tags.TryGetValue(tag, out var value) ? value : string.Empty).ToList();
    }

    internal class PollingStationData
    {
        public Guid Id { get; }
        public readonly Dictionary<string, string> Tags = new();

        private PollingStationData(PollingStationModel pollingStation)
        {
            Id = pollingStation.Id;
        }

        public static PollingStationData For(PollingStationModel pollingStation)
        {
            var pollingStationData = new PollingStationData(pollingStation);

            pollingStationData.WithTags(pollingStation.Tags);

            return pollingStationData;
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
