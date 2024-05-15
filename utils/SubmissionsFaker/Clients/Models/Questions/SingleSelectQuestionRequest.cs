namespace SubmissionsFaker.Clients.Models.Questions;

public class SingleSelectQuestionRequest : BaseQuestionRequest
{
    public List<SelectOptionRequest> Options { get; set; } = new();
}
