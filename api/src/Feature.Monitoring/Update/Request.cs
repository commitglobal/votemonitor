namespace Feature.Monitoring.Update;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid NgoId { get; set; }
    public bool AllowMultipleFormSubmission { get; set; }
}
