namespace Feature.Statistics.GetPlatformAdminStatistics;

public class Response
{
    public List<CountryHistogramPoint> CountriesHistogram { get; set; } = [];
    public ObserversStats Observers { get; set; } = new();
    public NgosStats Ngos { get; set; } = new();
    public int NumberOfElections { get; set; }
    public int NumberOfPollingStations { get; set; }
    public int NumberOfVisitedPollingStations { get; set; }
    public int NumberOfMinutesMonitoring { get; set; }
    public int NumberOfFormSubmissions { get; set; }
    public int NumberOfQuestionsAnswered { get; set; }
    public int NumberOfFlaggedAnswers { get; set; }
    public int NumberOfQuickReports { get; set; }
    public int NumberOfIncidentReports { get; set; }
    public int NumberOfCitizenReports { get; set; }
}
