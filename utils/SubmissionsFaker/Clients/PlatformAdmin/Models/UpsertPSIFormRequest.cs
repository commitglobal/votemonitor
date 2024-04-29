using SubmissionsFaker.Clients.Models.Questions;

namespace SubmissionsFaker.Clients.PlatformAdmin.Models;

public class UpsertPSIFormRequest
{
    public string DefaultLanguage { get; set; }
    public List<string> Languages { get; set; } = [];

    public List<BaseQuestionRequest> Questions { get; set; } = new();
}