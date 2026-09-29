using Feature.FormTemplates.ListAssignedTemplates;
using Vote.Monitor.Domain.Entities.ElectionRoundFormTemplateAggregate;

namespace Feature.FormTemplates.Specifications;

public sealed class ListAssignedFormTemplateSpecification : Specification<ElectionRoundFormTemplate, FormTemplateSlimModel>
{
    public ListAssignedFormTemplateSpecification(Request request)
    {
        Query
            .Where(x => x.ElectionRoundId == request.ElectionRoundId)
            .Include(x => x.FormTemplate)
            .OrderBy(x => x.FormTemplate.Code);

        Query
            .Select(x => new FormTemplateSlimModel
            {
                Id = x.FormTemplate.Id,
                Code = x.FormTemplate.Code,
                DefaultLanguage = x.FormTemplate.DefaultLanguage,
                Name = x.FormTemplate.Name,
                Description = x.FormTemplate.Description,
                FormType = x.FormTemplate.FormType,
                Status = x.FormTemplate.Status,
                LastModifiedOn = x.FormTemplate.LastModifiedOn ?? x.FormTemplate.CreatedOn,
                Languages = x.FormTemplate.Languages
            });
    }
}
