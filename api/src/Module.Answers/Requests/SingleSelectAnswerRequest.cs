namespace Module.Answers.Requests;

public class SingleSelectAnswerRequest : BaseAnswerRequest
{
    public SelectedOptionRequest Selection { get; set; }
}
