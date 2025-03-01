namespace Feature.FormTemplates.Specifications;

public sealed class ListFormTemplatesByIds: Specification<FormTemplateAggregate>
{
    public ListFormTemplatesByIds(List<Guid> formTemplateIds)
    {
        Query.Where(formTemplate => formTemplateIds.Contains(formTemplate.Id));
    }
}
