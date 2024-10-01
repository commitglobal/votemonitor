namespace Vote.Monitor.Hangfire.Jobs.Export.QuickReports;

public class QuickReportsDataTable
{
    private readonly List<string> _header;
    private readonly List<List<object>> _dataTable;

    private QuickReportsDataTable()
    {
        _header = new List<string>();
        _dataTable = new List<List<object>>();

        _header.AddRange([
            "QuickReportId",
            "TimeSubmitted",
            "FollowUpStatus",
            "MonitoringObserverId",
            "FirstName",
            "LastName",
            "Email",
            "PhoneNumber",
            "LocationType",
            "Level1",
            "Level2",
            "Level3",
            "Level4",
            "Level5",
            "LevelNumber",
            "PollingStationDetails",
            "Title",
            "Description"
        ]);
    }

    public static QuickReportsDataTable New()
    {
        return new QuickReportsDataTable();
    }

    public QuickReportsDataTableGenerator WithData()
    {
        return QuickReportsDataTableGenerator.For(_header, _dataTable);
    }
}
