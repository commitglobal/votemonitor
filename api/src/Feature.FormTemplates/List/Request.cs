using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase;

namespace Feature.FormTemplates.List;

public class Request : BaseSortPaginatedRequest
{
    [QueryParam] public string? SearchText { get; set; }

    /// <summary>Only templates with any of these statuses (repeat the key for several).</summary>
    [QueryParam] public FormStatus[]? FormStatus { get; set; }

    /// <summary>Only templates of any of these types (repeat the key for several).</summary>
    [QueryParam] public FormType[]? FormTemplateType { get; set; }
}
