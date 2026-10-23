namespace Feature.Review.SetReviewers;

public class Request : ReportRequest
{
    /// <summary>Everyone who should review the record afterwards; the current reviewers are replaced.</summary>
    public Guid[] UserIds { get; set; } = [];
}
