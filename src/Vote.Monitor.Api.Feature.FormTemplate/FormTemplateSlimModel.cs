using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;

namespace Vote.Monitor.Api.Feature.FormTemplate;

public record FormTemplateSlimModel
{
    public Guid Id { get; init; }
    public required string Code { get; init; }
    public required string DefaultLanguage { get; init; }
    public List<string> Languages { get; init; } = [];

    public required TranslatedString Name { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<FormTemplateStatus, string>))]
    public required FormTemplateStatus Status { get; init; }

    public required DateTime CreatedOn { get; init; }
    public required DateTime? LastModifiedOn { get; init; }
}
