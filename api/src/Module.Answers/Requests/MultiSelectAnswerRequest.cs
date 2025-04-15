namespace Module.Answers.Requests;

public class MultiSelectAnswerRequest : BaseAnswerRequest
{
    public List<SelectedOptionRequest> Selection { get; set; } = [];
}
