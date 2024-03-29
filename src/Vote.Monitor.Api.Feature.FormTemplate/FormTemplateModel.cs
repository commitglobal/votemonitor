using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;
using Vote.Monitor.Form.Module.Models;

namespace Vote.Monitor.Api.Feature.FormTemplate;

public record FormTemplateModel
{
    public Guid Id { get; init; }
    public required string Code { get; init; }
    public required TranslatedString Name { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<FormTemplateStatus, string>))]
    public required FormTemplateStatus Status { get; init; }

    public required DateTime CreatedOn { get; init; }
    public required DateTime? LastModifiedOn { get; init; }
    public List<BaseQuestionModel> Questions { get; init; } = [];
    public List<string> Languages { get; init; } = [];
}
