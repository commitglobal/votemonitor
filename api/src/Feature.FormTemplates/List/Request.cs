using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Feature.FormTemplates.List;

public class Request : BaseSortPaginatedRequest
{
    [QueryParam] public string? SearchText { get; set; }

    [QueryParam] public FormStatus? FormStatus { get; set; }

    [QueryParam] public FormType? FormTemplateType { get; set; }
}
