using Vote.Monitor.Domain.Entities.FormTemplateAggregate;

namespace Vote.Monitor.Api.Feature.FormTemplate.Specifications;

public sealed class GetFormTemplateSpecification : Specification<FormTemplateAggregate>
{
    public GetFormTemplateSpecification(Guid id, string code, FormType formType)
    {
        Query
            .Where(x => x.Id != id && x.Code == code && x.FormType == formType);
    }

    public GetFormTemplateSpecification(string code, FormType formType)
    {
        Query
            .Where(x => x.Code == code && x.FormType == formType);
    }
}
