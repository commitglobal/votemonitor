using Vote.Monitor.Form.Module.Requests;

namespace Vote.Monitor.Api.Feature.PollingStation.InformationForm.Update;

public class Request
{
    public Guid Id { get; set; }
    public List<string> Languages { get; set; } = new();
    public List<BaseQuestionRequest> Questions { get; set; } = new();
}
