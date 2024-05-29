namespace Feature.Feedback.List;

public record Response
{
    public List<FeedbackModel> Feedback { get; set; }
}
