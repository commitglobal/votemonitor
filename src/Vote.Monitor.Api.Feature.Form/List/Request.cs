using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;

namespace Vote.Monitor.Api.Feature.Form.List;

public class Request: BaseSortPaginatedRequest
{
    public Guid ElectionRoundId { get; set; }
    public Guid MonitoringNgoId { get; set; }

    [QueryParam]
    public string? CodeFilter { get; set; }

    [QueryParam]
    public Guid? LanguageId { get; set; }

    [QueryParam]
    [JsonConverter(typeof(SmartEnumNameConverter<FormTemplateStatus, string>))]
    public FormTemplateStatus? Status { get; set; }
}
