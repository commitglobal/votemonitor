using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Form.Module.Requests;

public class SectionRequest
{
    public string Code { get; set; }
    public TranslatedString Title { get; set; }
    public List<BaseQuestionRequest> Questions { get; set; } = new();
}
