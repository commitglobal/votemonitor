namespace Feature.Statistics.Get;

public class Response
{
    public ObserversStats ObserversStats { get; set; }
    public int NumberOfObserversOnTheField { get; set; }
    public PollingStationStats PollingStationsStats { get; set; }
    public decimal MinutesMonitoring { get; set; }
    public HistogramPoint[] FormsHistogram { get; set; } = [];
    public HistogramPoint[] QuestionsHistogram { get; set; } = [];
    public HistogramPoint[] FlaggedAnswersHistogram { get; set; } = [];
    public HistogramPoint[] QuickReportsHistogram { get; set; } = [];
}
