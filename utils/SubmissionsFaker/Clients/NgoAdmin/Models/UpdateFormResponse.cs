using SubmissionsFaker.Clients.Models;
using SubmissionsFaker.Clients.Models.Questions;

namespace SubmissionsFaker.Clients.NgoAdmin.Models;

public class UpdateFormResponse : ResponseWithId
{
    public List<BaseQuestionRequest> Questions { get; set; } = new();
}