namespace SubmissionsFaker.Clients.NgoAdmin.Models;

public class MultiSelectQuestionRequest : BaseQuestionRequest
{
    public List<SelectOptionRequest> Options { get; set; } = new();
}
