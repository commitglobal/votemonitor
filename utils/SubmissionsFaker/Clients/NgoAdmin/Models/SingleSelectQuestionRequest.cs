namespace SubmissionsFaker.Clients.NgoAdmin.Models;

public class SingleSelectQuestionRequest : BaseQuestionRequest
{
    public List<SelectOptionRequest> Options { get; set; } = new();
}
