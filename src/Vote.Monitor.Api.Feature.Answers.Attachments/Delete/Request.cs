namespace Vote.Monitor.Api.Feature.Answers.Attachments.Delete;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid PollingStationId { get; set; }
    public Guid FormId { get; set; }
    public Guid QuestionId { get; set; }

    [FromClaim("Sub")]
    public Guid ObserverId { get; set; }

    public Guid Id { get; set; }
}
