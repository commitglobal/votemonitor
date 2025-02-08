using Vote.Monitor.Domain.Entities.FormTemplateAggregate;
using Vote.Monitor.Domain.Specifications;

namespace Feature.FormTemplates.Specifications;

public sealed class ListFormTemplatesSpecification : Specification<FormTemplateAggregate, FormTemplateSlimModel>
{
    public ListFormTemplatesSpecification(List.Request request, bool isNgoAdmin)
    {
        Query
            .Where(x => x.Status == request.FormTemplateStatus, request.FormTemplateStatus != null && !isNgoAdmin)
            .Where(x => x.Status == FormTemplateStatus.Published, isNgoAdmin)
            .Where(x => x.FormType == request.FormTemplateType, !isNgoAdmin)
            .ApplyOrdering(request)
            .Paginate(request);

        Query.Select(x => new FormTemplateSlimModel
        {
            Id = x.Id,
            FormType = x.FormType,
            Code = x.Code,
            DefaultLanguage = x.DefaultLanguage,
            Name = x.Name,
            Description = x.Description,
            Status = x.Status,
            Languages = x.Languages,
            CreatedOn = x.CreatedOn,
            LastModifiedOn = x.LastModifiedOn,
            NumberOfQuestions = x.NumberOfQuestions
        });
    }
}
