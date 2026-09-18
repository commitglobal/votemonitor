using Feature.Statistics.GetNgoAdminStatistics.Models;

namespace Feature.Statistics.GetMonitoringObserverStatistics;

public class Response
{
    public VisitedPollingStationLevelStats? TotalStats { get; set; }
    public List<VisitedPollingStationLevelStats> Level1Stats { get; set; } = [];
    public List<VisitedPollingStationLevelStats> Level2Stats { get; set; } = [];
    public List<VisitedPollingStationLevelStats> Level3Stats { get; set; } = [];
    public List<VisitedPollingStationLevelStats> Level4Stats { get; set; } = [];
    public List<VisitedPollingStationLevelStats> Level5Stats { get; set; } = [];

    public int NumberOfFormsSubmitted { get; set; }
    public int NumberOfQuestionsAnswered { get; set; }
    public int NumberOfFlaggedAnswers { get; set; }
    public int NumberOfQuickReports { get; set; }
    public int NumberOfIncidentReports { get; set; }
    public int NumberOfNotes { get; set; }
    public int NumberOfAttachments { get; set; }
    public int NumberOfPollingStationsVisited { get; set; }
    public double MinutesMonitoring { get; set; }
}
