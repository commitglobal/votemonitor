using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;

namespace Feature.FormTemplates.List;

public class Request: BaseSortPaginatedRequest
{
    [QueryParam]
    public string? CodeFilter { get; set; }

    [QueryParam]
    public string? LanguageCode { get; set; }

    [QueryParam]
    [JsonConverter(typeof(SmartEnumNameConverter<FormTemplateStatus, string>))]
    public FormTemplateStatus? Status { get; set; }
}
