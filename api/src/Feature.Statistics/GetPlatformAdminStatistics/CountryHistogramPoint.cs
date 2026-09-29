namespace Feature.Statistics.GetPlatformAdminStatistics;

public class CountryHistogramPoint
{
    public Guid CountryId { get; set; }
    public string CountryName { get; set; } = string.Empty;
    public string Iso2 { get; set; } = string.Empty;
    public int NumberOfElections { get; set; }
}
