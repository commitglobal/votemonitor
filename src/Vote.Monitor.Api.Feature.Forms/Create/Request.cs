namespace Vote.Monitor.Api.Feature.Forms.Create;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public string Code { get; set; }
    public Guid LanguageId { get; set; }
    public string Description { get; set; }
}
