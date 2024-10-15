using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Form.Module.Mappers;
using Vote.Monitor.Form.Module.Models;

namespace Feature.Forms;

public class FormFullModel
{
    public TranslatedString Description { get; set; }

    public required Guid Id { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<FormType, string>))]
    public required FormType FormType { get; init; }

    public required string Code { get; init; }
    public TranslatedString Name { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<FormStatus, string>))]
    public required FormStatus Status { get; init; }

    public required string DefaultLanguage { get; init; }
    public required string[] Languages { get; init; } = [];
    public int NumberOfQuestions { get; init; }

    public LanguagesTranslationStatus LanguagesTranslationStatus { get; init; }

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
            LanguagesTranslationStatus = form.LanguagesTranslationStatus
        };

    public IReadOnlyList<BaseQuestionModel> Questions { get; init; } = [];
}