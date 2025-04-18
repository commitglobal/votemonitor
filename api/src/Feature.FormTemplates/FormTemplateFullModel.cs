using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase;
using Module.Forms.Models;

namespace Feature.FormTemplates;

public record FormTemplateFullModel
{
    public TranslatedString Description { get; init; }

    public required Guid Id { get; init; }

    public required FormType FormType { get; init; }

    public required string Code { get; init; }
    public TranslatedString Name { get; init; }

    public required FormStatus Status { get; init; }

    public required string DefaultLanguage { get; init; }
    public required string[] Languages { get; init; } = [];
    public int NumberOfQuestions { get; init; }
    public string? Icon { get; init; }

    public LanguagesTranslationStatus LanguagesTranslationStatus { get; init; }
    public IReadOnlyList<BaseQuestionModel> Questions { get; init; } = [];
    
    public static FormTemplateFullModel FromEntity(FormTemplateAggregate formTemplate) => formTemplate == null
        ? null
        : new FormTemplateFullModel
        {
            Id = formTemplate.Id,
            Code = formTemplate.Code,
            FormType = formTemplate.FormType,
            Status = formTemplate.Status,
            DefaultLanguage = formTemplate.DefaultLanguage,
            Languages = formTemplate.Languages,
            Name = formTemplate.Name,
            Questions = formTemplate.Questions.Select(QuestionsMapper.ToModel).ToList(),
            NumberOfQuestions = formTemplate.NumberOfQuestions,
            Description = formTemplate.Description,
            LanguagesTranslationStatus = formTemplate.LanguagesTranslationStatus,
            Icon = formTemplate.Icon,
        };
}
