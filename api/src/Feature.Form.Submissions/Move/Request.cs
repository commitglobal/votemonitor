namespace Feature.Form.Submissions.Move;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid SubmissionId { get; set; }

    public Guid PollingStationId { get; set; }
}
