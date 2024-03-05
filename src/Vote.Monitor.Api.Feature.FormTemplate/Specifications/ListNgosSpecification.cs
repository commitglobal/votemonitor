using Vote.Monitor.Api.Feature.FormTemplate.Models;
using Vote.Monitor.Domain.Specifications;

namespace Vote.Monitor.Api.Feature.FormTemplate.Specifications;

public class ListNgosSpecification : Specification<FormTemplateAggregate, FormTemplateModel>
{
    public ListNgosSpecification(List.Request request)
    {
        Query
            .Search(x => x.Code, "%" + request.CodeFilter + "%", !string.IsNullOrEmpty(request.CodeFilter))
            .Where(x => x.Status == request.Status, request.Status != null)
            .Include(x=>x.Languages)
            .ApplyOrdering(request)
            .Paginate(request);

        Query.Select(x => new FormTemplateModel
        {
            Id = x.Id,
            Name = x.Name,
            Status = x.Status,
            Languages = x.Languages.Select(x=>x.Iso1).ToList(),
            CreatedOn = x.CreatedOn,
            LastModifiedOn = x.LastModifiedOn,
        });
    }
}
