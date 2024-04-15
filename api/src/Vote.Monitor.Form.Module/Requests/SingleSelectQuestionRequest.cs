namespace Vote.Monitor.Form.Module.Requests;

public class SingleSelectQuestionRequest : BaseQuestionRequest
{
    public List<SelectOptionRequest> Options { get; set; } = new();
}
