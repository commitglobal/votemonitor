namespace Feature.Review.Stages.Delete;

public class Request : StagesRequest
{
    public Guid Id { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid UserId { get; set; }

    /// <summary>Where the reports in the deleted stage go. Required when the stage is in use.</summary>
    [QueryParam]
    public Guid? MoveTo { get; set; }
}
