namespace Vote.Monitor.Api.Feature.FormTemplate.Update.Requests;

public class SingleSelectQuestionRequest : BaseQuestionRequest
{
    public string Code { get; set; }
    public List<SelectOptionRequest> Options { get; set; }
}
