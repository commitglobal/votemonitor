using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase;
using Module.Forms.Mappers;
using Module.Forms.Models;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

namespace Feature.Forms.Models;

public class FormFullModel
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
    public IReadOnlyList<BaseQuestionModel> Questions { get; init; } = [];

    public LanguagesTranslationStatus LanguagesTranslationStatus { get; init; }

    public int DisplayOrder { get; init; }

    
    public static FormFullModel FromEntity(FormAggregate form) => form == null
        ? null
        : new FormFullModel
        {
            Id = form.Id,
            Code = form.Code,
            FormType = form.FormType,
            Status = form.Status,
            DefaultLanguage = form.DefaultLanguage,
            Languages = form.Languages,
            Name = form.Name,
            Questions = form.Questions.Select(QuestionsMapper.ToModel).ToList(),
            NumberOfQuestions = form.NumberOfQuestions,
            Description = form.Description,
            LanguagesTranslationStatus = form.LanguagesTranslationStatus,
            Icon = form.Icon,
            DisplayOrder = form.DisplayOrder
        };
    
    public static FormFullModel FromEntity(PollingStationInformationForm form) => form == null
        ? null
        : new FormFullModel
        {
            Id = form.Id,
            Code = form.Code,
            FormType = form.FormType,
            Status = form.Status,
            DefaultLanguage = form.DefaultLanguage,
            Languages = form.Languages,
            Name = form.Name,
            Questions = form.Questions.Select(QuestionsMapper.ToModel).ToList(),
            NumberOfQuestions = form.NumberOfQuestions,
            Description = form.Description,
            LanguagesTranslationStatus = form.LanguagesTranslationStatus,
            Icon = form.Icon,
        };
}
