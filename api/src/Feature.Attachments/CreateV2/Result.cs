namespace Feature.Attachments.CreateV2;

public class Result
{
    public Guid Id { get; set; }
    public string PresignedUrl { get; set; }
    public int UrlValidityInSeconds { get; set; }
    public Guid FormId { get; set; }
    public Guid PollingStationId { get; set; }
    public Guid QuestionId { get; set; }
}
