namespace Feature.Observers.Specifications;

public sealed class GetObserverByIdSpecification : SingleResultSpecification<ObserverAggregate>
{
    public GetObserverByIdSpecification(Guid id)
    {
        Query.Where(x => x.Id == id);
    }
}
