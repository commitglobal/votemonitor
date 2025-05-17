namespace Vote.Monitor.Hangfire.Jobs.Export.PollingStations;

public class PollingStationsDataTable
{
    private readonly List<string> _header;
    private readonly List<List<object>> _dataTable;

    private PollingStationsDataTable()
    {
        _header = new List<string>();
        _dataTable = new List<List<object>>();

        _header.AddRange([
            "Id",
            "Level1",
            "Level2",
            "Level3",
            "Level4",
            "Level5",
            "Number",
            "Address",
            "Latitude",
            "Longitude",
            "DisplayOrder",
        ]);
    }

    public static PollingStationsDataTable New()
    {
        return new PollingStationsDataTable();
    }

    public PollingStationsDataTableGenerator WithData()
    {
        return PollingStationsDataTableGenerator.For(_header, _dataTable);
    }
}
