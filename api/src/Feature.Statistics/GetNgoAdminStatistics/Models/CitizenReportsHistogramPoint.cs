namespace Feature.Statistics.GetNgoAdminStatistics.Models;

public class CitizenReportsHistogramPoint
{
    public DateTime Bucket { get; set; }
    public int NumberOfSubmissions { get; set; }
    public int NumberOfAnswers { get; set; }
    public int NumberOfFlaggedAnswers { get; set; }
}
