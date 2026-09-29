namespace Feature.Statistics.GetNgoAdminStatistics.Models;

public class VisitedLocationLevelStats
{
    public string Path { get; set; } = string.Empty;
    public int Level { get; set; }
    public int NumberOfVisitedLocations { get; set; }
    public int NumberOfLocations { get; set; }
    public int NumberOfCitizenReports { get; set; }
    public int NumberOfFlaggedAnswers { get; set; }
    public int NumberOfQuestionsAnswered { get; set; }
    public double CoveragePercentage { get; set; }
}
