using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;

namespace Feature.FormTemplates.Specifications;

public sealed class GetFormTemplateSpecification : SingleResultSpecification<FormTemplateAggregate>
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

    public GetFormTemplateSpecification(Guid id, bool isNgoAdmin)
    {
        Query
            .Where(x => x.Id == id)
            .Where(x => x.Status == FormTemplateStatus.Published, isNgoAdmin);
    }
}
