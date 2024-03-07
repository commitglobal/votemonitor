namespace Vote.Monitor.Api.Feature.FormTemplate.Update.Requests;

public class SingleSelectQuestionRequest : BaseQuestionRequest
{
    public List<SelectOptionRequest> Options { get; set; } = new();
}
