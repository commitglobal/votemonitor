namespace Feature.Statistics.GetElectionRoundStatistics;

public class NgosStats
{
    public int ActiveNgos { get; set; }
    public int InactiveNgos { get; set; }

    public int TotalNumberOfNgos => ActiveNgos + InactiveNgos;
}
