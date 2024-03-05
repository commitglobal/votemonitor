namespace Vote.Monitor.Api.Feature.FormTemplate.Specifications;

public sealed class GetByIdSpecification : Specification<FormTemplateAggregate>, ISingleResultSpecification<FormTemplateAggregate>
{
    public GetByIdSpecification(Guid id)
    {
        Query
            .Where(x => x.Id == id)
            .Include(x => x.Languages);
    }
}
