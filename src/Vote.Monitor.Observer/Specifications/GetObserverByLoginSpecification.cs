using Ardalis.Specification;

namespace Vote.Monitor.Observer.Specifications;

public class GetObserverByLoginSpecification : Specification<Domain.Entities.ApplicationUserAggregate.Observer>, ISingleResultSpecification<Domain.Entities.ApplicationUserAggregate.Observer>
{
    public GetObserverByLoginSpecification(string login)
    {
        Query.Where(x => x.Login == login);
    }
}
