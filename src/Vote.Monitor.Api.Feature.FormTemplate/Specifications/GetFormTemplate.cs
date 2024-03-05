using Vote.Monitor.Domain.Entities.FormTemplateAggregate;

namespace Vote.Monitor.Api.Feature.FormTemplate.Specifications;

public class GetFormTemplate : Specification<FormTemplateAggregate>
{
    public GetFormTemplate(Guid id, string code, FormType formType)
    {
        Query
            .Where(x => x.Id != id && x.Code == code && x.FormType == formType);
    }

    public GetFormTemplate(string code, FormType formType)
    {
        Query
            .Where(x => x.Code == code && x.FormType == formType);
    }
}
