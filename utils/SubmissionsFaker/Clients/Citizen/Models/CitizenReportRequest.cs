using SubmissionsFaker.Clients.MonitoringObserver.Models;

namespace SubmissionsFaker.Clients.Citizen.Models;

public class CitizenReportRequest
{
    public string CitizenReportId { get; set; }
    public string FormId { get; set; }
    public List<BaseAnswerRequest> Answers { get; set; } = [];
    public Guid LocationId { get; set; }
}