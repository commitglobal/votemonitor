namespace Vote.Monitor.Hangfire.Jobs.Export.Locations;

public class LocationsDataTable
{
    private readonly List<string> _header;
    private readonly List<List<object>> _dataTable;

    private LocationsDataTable()
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
            "DisplayOrder"
        ]);
    }

    public static LocationsDataTable New()
    {
        return new LocationsDataTable();
    }

    public LocationsDataTableGenerator WithData()
    {
        return LocationsDataTableGenerator.For(_header, _dataTable);
    }
}
