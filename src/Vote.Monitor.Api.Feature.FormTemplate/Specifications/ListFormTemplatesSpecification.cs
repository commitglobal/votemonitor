using Vote.Monitor.Api.Feature.FormTemplate.Models;
using Vote.Monitor.Domain.Specifications;

namespace Vote.Monitor.Api.Feature.FormTemplate.Specifications;

public sealed class ListFormTemplatesSpecification : Specification<FormTemplateAggregate, FormTemplateModel>
{
    public ListFormTemplatesSpecification(List.Request request)
    {
        Query
            .Search(x => x.Code, "%" + request.CodeFilter + "%", !string.IsNullOrEmpty(request.CodeFilter))
            .Where(x => x.Status == request.Status, request.Status != null)
            .ApplyOrdering(request)
            .Paginate(request);

        Query.Select(x => new FormTemplateModel
        {
            Id = x.Id,
            Code = x.Code,
            Name = x.Name,
            Status = x.Status,
            Languages = x.Languages.ToList(),
            CreatedOn = x.CreatedOn,
            LastModifiedOn = x.LastModifiedOn,
        });
    }
}
