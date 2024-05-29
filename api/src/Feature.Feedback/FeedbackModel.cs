namespace Feature.Feedback;

public class FeedbackModel
{
    public Guid Id { get; init; }
    public Guid ElectionRoundId { get; init; }
    public Guid ObserverId { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string UserFeedback { get; init; }
    public DateTime TimeSubmitted { get; init; }
}
