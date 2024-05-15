namespace Feature.Statistics.Get;

public class BucketView
{
    public DateTime Bucket { get; set; }
    public int FormsSubmitted { get; set; }
    public int NumberOfQuestionsAnswered { get; set; }
    public int NumberOfFlaggedAnswers { get; set; }
    public int PollingStationNotesCount { get; set; }
    public int PollingStationAttachmentsCount { get; set; }
    public int PollingStationsInformationCount { get; set; }
    public int VisitedPollingStations { get; set; }
    public int DistinctLevel1 { get; set; }
    public int DistinctLevel2 { get; set; }
    public int DistinctLevel3 { get; set; }
    public int DistinctLevel4 { get; set; }
    public int DistinctLevel5 { get; set; }
}
