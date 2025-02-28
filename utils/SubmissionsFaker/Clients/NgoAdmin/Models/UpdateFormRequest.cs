using SubmissionsFaker.Clients.Models.Questions;

namespace SubmissionsFaker.Clients.NgoAdmin.Models;

public record UpdateFormRequest : CreateFormRequest
{
    public List<BaseQuestionRequest> Questions { get; set; } = new();
    public Guid Id { get; set; }
}