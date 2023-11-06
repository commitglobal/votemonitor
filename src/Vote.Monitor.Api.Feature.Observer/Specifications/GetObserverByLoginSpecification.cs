namespace Vote.Monitor.Api.Feature.Observer.Specifications;

public class GetObserverByLoginSpecification : Specification<ObserverAggregate>, ISingleResultSpecification<ObserverAggregate>
{
    public GetObserverByLoginSpecification(string login)
    {
        Query.Where(x => x.Login == login);
    }
}
