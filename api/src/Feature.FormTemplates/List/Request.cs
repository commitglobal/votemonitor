using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;

namespace Feature.FormTemplates.List;

public class Request: BaseSortPaginatedRequest
{
    [QueryParam]
    public string? SearchText { get; set; }

    [QueryParam]
    public FormTemplateStatus? FormTemplateStatus { get; set; }
    
    [QueryParam]
    public FormType? FormTemplateType { get; set; }
}
