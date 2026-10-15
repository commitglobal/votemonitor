namespace Feature.Statistics.GetNgoAdminStatistics.Models;

public class AnswersHistogramPoint
{
    public DateTime Bucket { get; set; }
    public int NumberOfAnswers { get; set; }
    public int NumberOfFlaggedAnswers { get; set; }
}
