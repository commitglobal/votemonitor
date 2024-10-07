namespace Feature.CitizenReports.Notes.Delete;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid CitizenReportId { get; set; }
    public Guid Id { get; set; }
}
