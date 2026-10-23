namespace Feature.Review;

/// <summary>Route and claims shared by the endpoints that act on one report.</summary>
public class ReportRequest
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid UserId { get; set; }

    public ReportRecordType RecordType { get; set; } = null!;

    public Guid RecordId { get; set; }
}

public class ReportRequestValidator<T> : Validator<T> where T : ReportRequest
{
    public ReportRequestValidator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.NgoId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.RecordType).NotNull();
        RuleFor(x => x.RecordId).NotEmpty();
    }
}
