using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase;

namespace Feature.FormTemplates.Specifications;

public sealed class GetFormTemplateReadOnlySpecification : SingleResultSpecification<FormTemplateAggregate>
{
    public GetFormTemplateReadOnlySpecification(Guid id, string code, FormType formType)
    {
        Query.AsNoTracking();

        Query
            .Where(x => x.Id != id && x.Code == code && x.FormType == formType);
    }

    public GetFormTemplateReadOnlySpecification(string code, FormType formType)
    {
        Query.AsNoTracking();

        Query
            .Where(x => x.Code == code && x.FormType == formType);
    }

    public GetFormTemplateReadOnlySpecification(Guid id, bool isNgoAdmin)
    {
        Query.AsNoTracking();

        Query
            .Where(x => x.Id == id)
            .Where(x => x.Status == FormStatus.Published, isNgoAdmin);
    }
}
