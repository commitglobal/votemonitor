namespace Feature.Statistics.GetNgoAdminStatistics.Models;

public class VisitedPollingStationLevelStats
{
    // Represents the concatenated path from Level1, Level2, Level3, Level4, Level5
    public string Path { get; set; }
    public int NumberOfVisitedPollingStations { get; set; }
    public int NumberOfPollingStations { get; set; }
    public int NumberOfIncidentReports { get; set; }
    public int NumberOfQuickReports { get; set; }
    public int NumberOfFormSubmissions { get; set; }
    public double MinutesMonitoring { get; set; }
    public int NumberOfFlaggedAnswers { get; set; }
    public int NumberOfQuestionsAnswered { get; set; }
    public int ActiveObservers { get; set; }
    public int Level { get; set; }
    public double CoveragePercentage { get; set; }
}