namespace Feature.PollingStationCorrections.ListRecords;

public class Request : BaseSortPaginatedRequest
{
    public Guid ElectionRoundId { get; set; }

    [QueryParam] public string? SearchText { get; set; }

    /// <summary>Only records of any of these types (repeat the key for several).</summary>
    [QueryParam] public CorrectableRecordType[]? RecordType { get; set; }

    [QueryParam] public Guid? MonitoringNgoId { get; set; }

    /// <summary>Part of the polling station number, case-insensitive ("01/" finds 01/001, 01/002…).</summary>
    [QueryParam] public string? PollingStationNumberFilter { get; set; }

    [QueryParam] public bool? CorrectedOnly { get; set; }

    // Each level filter keeps the records matching any of its values (repeat the key for several).
    [QueryParam] public string[]? Level1Filter { get; set; }
    [QueryParam] public string[]? Level2Filter { get; set; }
    [QueryParam] public string[]? Level3Filter { get; set; }
    [QueryParam] public string[]? Level4Filter { get; set; }
    [QueryParam] public string[]? Level5Filter { get; set; }
}
