using Vote.Monitor.Core.RulesEngine.Rules;

namespace Feature.Review.StageCounts;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    public ReportRecordType RecordType { get; set; } = null!;

    /// <summary>Ignored for citizen reports, which have no coalition view.</summary>
    public DataSource DataSource { get; set; } = DataSource.Ngo;

    /// <summary>The same filter as the report list, so the counts match it.</summary>
    public FilterRule? Filter { get; set; }
}

public record StageCountModel(Guid StageId, int Count);
