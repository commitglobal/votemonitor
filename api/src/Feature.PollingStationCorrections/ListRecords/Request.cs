namespace Feature.PollingStationCorrections.ListRecords;

public class Request : BaseSortPaginatedRequest
{
    public Guid ElectionRoundId { get; set; }

    [QueryParam] public string? SearchText { get; set; }

    [QueryParam] public CorrectableRecordType? RecordType { get; set; }

    [QueryParam] public Guid? MonitoringNgoId { get; set; }

    [QueryParam] public string? PollingStationNumberFilter { get; set; }

    [QueryParam] public bool? CorrectedOnly { get; set; }

    [QueryParam] public string? Level1Filter { get; set; }
    [QueryParam] public string? Level2Filter { get; set; }
    [QueryParam] public string? Level3Filter { get; set; }
    [QueryParam] public string? Level4Filter { get; set; }
    [QueryParam] public string? Level5Filter { get; set; }
}
