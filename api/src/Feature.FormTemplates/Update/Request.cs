using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormBase;
using Vote.Monitor.Form.Module.Requests;

namespace Feature.FormTemplates.Update;

public class Request
{
    public Guid Id { get; set; }
    public string DefaultLanguage { get; set; }
    public string Code { get; set; }
    public TranslatedString Name { get; set; } = new();
    public TranslatedString Description { get; set; } = new();
    public FormType FormType { get; set; }
    public List<string> Languages { get; set; } = new();
    public string? Icon { get; set; }
    public List<BaseQuestionRequest> Questions { get; set; } = new();
}
