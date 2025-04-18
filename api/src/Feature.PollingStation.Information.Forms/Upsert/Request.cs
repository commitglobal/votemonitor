using Module.Forms.Requests;

namespace Feature.PollingStation.Information.Forms.Upsert;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public string DefaultLanguage { get; set; }
    public List<string> Languages { get; set; } = new();
    public List<BaseQuestionRequest> Questions { get; set; } = new();
}
