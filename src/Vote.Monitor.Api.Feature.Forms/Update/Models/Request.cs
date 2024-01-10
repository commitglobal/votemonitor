namespace Vote.Monitor.Api.Feature.Forms.Update.Models;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid Id { get; set; }
    public Guid LanguageId { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }

    public List<BaseQuestionRequest> Questions { get; set; } = new();
}
