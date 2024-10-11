using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Feature.Forms;

public class FormSlimModel
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
    public required DateTime LastModifiedOn { get; init; }

    public string LastModifiedBy { get; init; }

    public LanguagesTranslationStatus LanguagesTranslationStatus { get; init; }
}