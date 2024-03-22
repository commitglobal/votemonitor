using Vote.Monitor.Form.Module.Requests;

namespace Feature.PollingStation.Information.Form.Upsert;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public List<string> Languages { get; set; } = new();
    public List<BaseQuestionRequest> Questions { get; set; } = new();

}
