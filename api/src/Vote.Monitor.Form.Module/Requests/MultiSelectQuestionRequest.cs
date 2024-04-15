namespace Vote.Monitor.Form.Module.Requests;

public class MultiSelectQuestionRequest : BaseQuestionRequest
{
    public List<SelectOptionRequest> Options { get; set; } = new();
}
