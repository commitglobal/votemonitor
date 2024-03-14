namespace Vote.Monitor.Api.Feature.Observer.Specifications;

public sealed class GetObserversByLoginsSpecification : Specification<ObserverAggregate>
{
    public GetObserversByLoginsSpecification(IEnumerable<string> logins)
    {
        Query.Where(x => logins.Contains(x.Login));
    }
}
