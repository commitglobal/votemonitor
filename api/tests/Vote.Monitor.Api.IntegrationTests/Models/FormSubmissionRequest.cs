using Vote.Monitor.Answer.Module.Requests;

namespace Vote.Monitor.Api.IntegrationTests.Models;

public class FormSubmissionRequest
{
    public Guid PollingStationId { get; set; }
    public Guid FormId { get; set; }

    public List<BaseAnswerRequest>? Answers { get; set; }
    public bool? IsCompleted { get; set; }
}
