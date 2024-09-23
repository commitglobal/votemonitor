namespace Feature.CitizenReports.Guides.Update;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? Text  { get; set; }
    public string? WebsiteUrl  { get; set; }
}
