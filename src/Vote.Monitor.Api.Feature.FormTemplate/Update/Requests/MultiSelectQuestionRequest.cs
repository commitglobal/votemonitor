namespace Vote.Monitor.Api.Feature.FormTemplate.Update.Requests;

public class MultiSelectQuestionRequest : BaseQuestionRequest
{
    public List<SelectOptionRequest> Options { get; set; } = new();
}
