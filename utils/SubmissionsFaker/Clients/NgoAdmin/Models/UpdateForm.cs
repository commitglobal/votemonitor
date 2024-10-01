using SubmissionsFaker.Clients.Models.Questions;

namespace SubmissionsFaker.Clients.NgoAdmin.Models;

public record UpdateForm : NewForm
{
    public List<BaseQuestionRequest> Questions { get; set; } = new();
}