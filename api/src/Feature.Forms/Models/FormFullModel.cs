using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase;
using Module.Forms.Mappers;
using Module.Forms.Models;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

namespace Feature.Forms.Models;

public class FormFullModel
{
    public TranslatedString Description { get; set; }

    public required Guid Id { get; init; }

    public required FormType FormType { get; init; }

    public required string Code { get; init; }
    public TranslatedString Name { get; set; }

    public required FormStatus Status { get; init; }

    public required string DefaultLanguage { get; init; }
    public required string[] Languages { get; init; } = [];
    public int NumberOfQuestions { get; init; }
    public string? Icon { get; init; }
    public BaseQuestionModel[] Questions { get; set; } = [];

    public LanguagesTranslationStatus LanguagesTranslationStatus { get; set; }

    public int DisplayOrder { get; init; }

    public required DateTime LastModifiedOn { get; init; }

    public string LastModifiedBy { get; set; } = string.Empty;

    public bool IsFormOwner { get; init; }

    public static FormFullModel FromEntity(FormAggregate form) => FromEntity(form, string.Empty, isFormOwner: true);

    public static FormFullModel FromEntity(FormAggregate form, string lastModifiedBy) =>
        FromEntity(form, lastModifiedBy, isFormOwner: true);

    public static FormFullModel FromEntity(FormAggregate form, string lastModifiedBy, bool isFormOwner) => form == null
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
            Questions = form.Questions.Select(QuestionsMapper.ToModel).ToArray(),
            NumberOfQuestions = form.NumberOfQuestions,
            Description = form.Description,
            LanguagesTranslationStatus = form.LanguagesTranslationStatus,
            Icon = form.Icon,
            DisplayOrder = form.DisplayOrder,
            LastModifiedOn = form.LastModifiedOn ?? form.CreatedOn,
            LastModifiedBy = lastModifiedBy,
            IsFormOwner = isFormOwner
        };

    public static FormFullModel FromEntity(PollingStationInformationForm form) =>
        FromEntity(form, string.Empty);

    public static FormFullModel FromEntity(PollingStationInformationForm form, string lastModifiedBy) => form == null
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
            Questions = form.Questions.Select(QuestionsMapper.ToModel).ToArray(),
            NumberOfQuestions = form.NumberOfQuestions,
            Description = form.Description,
            LanguagesTranslationStatus = form.LanguagesTranslationStatus,
            Icon = form.Icon,
            LastModifiedOn = form.LastModifiedOn ?? form.CreatedOn,
            LastModifiedBy = lastModifiedBy,
            IsFormOwner = false
        };
}
