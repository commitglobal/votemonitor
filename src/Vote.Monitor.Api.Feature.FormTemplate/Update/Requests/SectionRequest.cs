namespace Vote.Monitor.Api.Feature.FormTemplate.Update.Requests;

public class SectionRequest
{
    public string Code { get; set; }
    public TranslatedString Title { get; set; }
    public List<BaseQuestionRequest> Questions { get; set; } = new();
}
