using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

namespace Vote.Monitor.Api.Feature.Monitoring.Specifications;

public class GetObserverStatusSpecification : SingleResultSpecification<ObserverAggregate, UserStatus>
{
    public GetObserverStatusSpecification(Guid observerId)
    {
        Query
            .Where(x => x.Id == observerId);

        Query.Select(x => x.Status);
    }
}
