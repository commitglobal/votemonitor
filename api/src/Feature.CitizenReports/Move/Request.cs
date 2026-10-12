namespace Feature.CitizenReports.Move;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid CitizenReportId { get; set; }

    public Guid LocationId { get; set; }
}
