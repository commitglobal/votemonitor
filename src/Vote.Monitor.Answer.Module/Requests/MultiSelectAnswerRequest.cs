namespace Vote.Monitor.Answer.Module.Requests;

public class MultiSelectAnswerRequest : BaseAnswerRequest
{
    public List<SelectedOptionRequest> Selection { get; set; } = [];
}
