using Vote.Monitor.Domain.Entities.FormTemplateAggregate;

namespace Feature.FormTemplates.Specifications;

public sealed class GetFormTemplateSpecification : Specification<FormTemplateAggregate>
{
    public GetFormTemplateSpecification(Guid id, string code, FormTemplateType formTemplateType)
    {
        Query
            .Where(x => x.Id != id && x.Code == code && x.FormTemplateType == formTemplateType);
    }

    public GetFormTemplateSpecification(string code, FormTemplateType formTemplateType)
    {
        Query
            .Where(x => x.Code == code && x.FormTemplateType == formTemplateType);
    }
}
