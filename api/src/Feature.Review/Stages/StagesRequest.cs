namespace Feature.Review.Stages;

/// <summary>Route and claims shared by the stage endpoints: the caller's NGO's stages in a round.</summary>
public class StagesRequest
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }
}

internal static class StageOrdering
{
    private static readonly ReviewStatus[] StatusOrder =
        [ReviewStatus.NotReviewed, ReviewStatus.InReview, ReviewStatus.Reviewed];

    /// <summary>Statuses in review order (Not reviewed, In review, Reviewed), then by sequence and name.</summary>
    public static IEnumerable<ReviewStage> InReviewOrder(this IEnumerable<ReviewStage> stages) =>
        stages.OrderBy(x => Array.IndexOf(StatusOrder, x.Status)).ThenBy(x => x.Sequence).ThenBy(x => x.Name);
}
