namespace Feature.CitizenReports.Attachments.Get;

public class Request
{
    public  Guid ElectionRoundId { get; set; }

    public  Guid CitizenReportId { get; set; }
    public  Guid Id { get; set; }
}
