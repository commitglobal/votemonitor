namespace Vote.Monitor.Api.Feature.Observer.Specifications;

public class GetObserversByLoginsSpecification : Specification<ObserverAggregate>
{
    public GetObserversByLoginsSpecification(IEnumerable<string> logins)
    {
        Query.Where(x => logins.Contains(x.Login));
    }
}
