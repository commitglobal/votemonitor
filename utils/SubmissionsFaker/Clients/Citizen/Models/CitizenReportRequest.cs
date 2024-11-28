using SubmissionsFaker.Clients.MonitoringObserver.Models;

namespace SubmissionsFaker.Clients.Citizen.Models;

public class CitizenReportRequest
{
    public Guid CitizenReportId { get; set; }
    public Guid FormId { get; set; }
    public List<BaseAnswerRequest> Answers { get; set; } = [];
    public Guid LocationId { get; set; }
}