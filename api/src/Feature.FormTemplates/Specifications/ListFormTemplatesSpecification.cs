using Vote.Monitor.Domain.Entities.FormTemplateAggregate;
using Vote.Monitor.Domain.Specifications;

namespace Feature.FormTemplates.Specifications;

public sealed class ListFormTemplatesSpecification : Specification<FormTemplateAggregate, FormTemplateSlimModel>
{
    public ListFormTemplatesSpecification(List.Request request)
    {
        Query
            .Search(x => x.Code, "%" + request.CodeFilter + "%", !string.IsNullOrEmpty(request.CodeFilter))
            .Where(x => x.Status == request.Status || x.Status == FormTemplateStatus.Published, request.Status != null)
            .Where(x => x.DefaultLanguage == request.LanguageCode || x.Languages.Contains(request.LanguageCode),
                !string.IsNullOrWhiteSpace(request.LanguageCode))
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
