namespace Feature.Statistics.GetNgoAdminStatistics.Models;

public class FormSubmissionsHistogramPoint
{
    public DateTime Bucket { get; set; }
    public int FormsSubmitted { get; set; }
    public int NumberOfQuestionsAnswered { get; set; }
    public int NumberOfFlaggedAnswers { get; set; }
}
