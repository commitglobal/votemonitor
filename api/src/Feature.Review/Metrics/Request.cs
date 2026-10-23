namespace Feature.Review.Metrics;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    /// <summary>Like the report lists: <c>Ngo</c> (default) or, for coalition leaders, <c>Coalition</c>. Citizen reports ignore it.</summary>
    [QueryParam]
    public DataSource DataSource { get; set; } = DataSource.Ngo;

    /// <summary>When given (1 to 5), adds a breakdown by place down to this level.</summary>
    [QueryParam]
    public int? Level { get; set; }
}
