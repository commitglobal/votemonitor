using SubmissionsFaker.Clients.Models.Questions;

namespace SubmissionsFaker.Clients.NgoAdmin.Models;

public record CreateFormRequest
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public TranslatedString Name { get; set; } = new();
    public TranslatedString Description { get; set; } = new();

    /// <summary>
    /// Opening | Voting | ClosingAndCounting
    /// </summary>
    public string FormType { get; set; }

    public List<string> Languages { get; set; } = [];
    public string DefaultLanguage { get; set; }
    public string? Icon { get; set; }

    public List<BaseQuestionRequest> Questions { get; set; } = new();
}