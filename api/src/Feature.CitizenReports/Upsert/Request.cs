using Vote.Monitor.Answer.Module.Requests;

namespace Feature.CitizenReports.Upsert;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid CitizenReportId { get; set; }
    public Guid FormId { get; set; }
    public List<BaseAnswerRequest>? Answers { get; set; }
}
