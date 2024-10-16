namespace Feature.CitizenReports.SendCopy;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid CitizenReportId { get; set; }
    public Guid FormId { get; set; }
    public string Email { get; set; }
}