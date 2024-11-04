using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;

namespace Feature.FormTemplates;

public record FormTemplateSlimModel
{
    public Guid Id { get; init; }
    public required string Code { get; init; }
    public required string DefaultLanguage { get; init; }
    public string[] Languages { get; init; } = [];

    public required TranslatedString Name { get; init; }
    public required TranslatedString Description { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<FormType, string>))]
    public required FormType FormType { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<FormTemplateStatus, string>))]
    public required FormTemplateStatus Status { get; init; }
    public int NumberOfQuestions { get; set; }
    public required DateTime CreatedOn { get; init; }
    public required DateTime? LastModifiedOn { get; init; }
}
