namespace SubmissionsFaker.Clients.Models.Questions;

public class MultiSelectQuestionRequest : BaseQuestionRequest
{
    public List<SelectOptionRequest> Options { get; set; } = new();
}
