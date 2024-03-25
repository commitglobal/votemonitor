namespace Vote.Monitor.Answer.Module.Requests;

public class SingleSelectAnswerRequest : BaseAnswerRequest
{
    public SelectedOptionRequest Selection { get; set; }
}
