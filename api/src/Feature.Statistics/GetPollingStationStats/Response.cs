namespace Feature.Statistics.GetPollingStationStats;

public class Response
{
    public int TotalAnswers { get; set; }
    public int FlaggedAnswers { get; set; }
    public int ObserversVisited { get; set; }
    public double MinutesMonitoring { get; set; }
    public int FormSubmissions { get; set; }
    public int QuickReports { get; set; }
    public int IncidentReports { get; set; }
}
