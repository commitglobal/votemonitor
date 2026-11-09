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

    /// <summary>
    /// Statuses in review order (Not reviewed, In review, Reviewed), then by sequence and name; each stage's
    /// sub-stages (in their own order) right after it.
    /// </summary>
    public static IEnumerable<ReviewStage> InReviewOrder(this IEnumerable<ReviewStage> stages)
    {
        var all = stages.ToList();
        IOrderedEnumerable<ReviewStage> Ordered(IEnumerable<ReviewStage> list) =>
            list.OrderBy(x => Array.IndexOf(StatusOrder, x.Status)).ThenBy(x => x.Sequence).ThenBy(x => x.Name);

        var ids = all.Select(x => x.Id).ToHashSet();
        // A sub-stage whose parent isn't in the list reads as a top-level stage.
        var top = all.Where(x => x.ParentStageId is null || !ids.Contains(x.ParentStageId.Value));
        foreach (var stage in Ordered(top))
        {
            yield return stage;
            foreach (var child in Ordered(all.Where(x => x.ParentStageId == stage.Id)))
            {
                yield return child;
            }
        }
    }
}
