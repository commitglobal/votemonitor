using Feature.Statistics.GetNgoAdminStatistics.Models;

namespace Feature.Statistics.GetElectionRoundStatistics;

public class Response
{
    public NgosStats NgosStats { get; set; } = new();
    public ObserversStats ObserversStats { get; set; } = new();

    public VisitedPollingStationLevelStats? TotalStats { get; set; }
    public List<VisitedPollingStationLevelStats> Level1Stats { get; set; } = [];
    public List<VisitedPollingStationLevelStats> Level2Stats { get; set; } = [];
    public List<VisitedPollingStationLevelStats> Level3Stats { get; set; } = [];
    public List<VisitedPollingStationLevelStats> Level4Stats { get; set; } = [];
    public List<VisitedPollingStationLevelStats> Level5Stats { get; set; } = [];

    public VisitedLocationLevelStats? TotalLocationStats { get; set; }
    public List<VisitedLocationLevelStats> LocationLevel1Stats { get; set; } = [];
    public List<VisitedLocationLevelStats> LocationLevel2Stats { get; set; } = [];
    public List<VisitedLocationLevelStats> LocationLevel3Stats { get; set; } = [];
    public List<VisitedLocationLevelStats> LocationLevel4Stats { get; set; } = [];
    public List<VisitedLocationLevelStats> LocationLevel5Stats { get; set; } = [];

    public HistogramPoint[] FormsHistogram { get; set; } = [];
    public HistogramPoint[] QuestionsHistogram { get; set; } = [];
    public HistogramPoint[] FlaggedAnswersHistogram { get; set; } = [];
    public HistogramPoint[] QuickReportsHistogram { get; set; } = [];
    public HistogramPoint[] IncidentReportsHistogram { get; set; } = [];
    public HistogramPoint[] CitizenReportsHistogram { get; set; } = [];
}
