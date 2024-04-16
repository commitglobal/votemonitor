using Vote.Monitor.Domain.Entities.NotificationTokenAggregate;

namespace Vote.Monitor.Api.Feature.Notifications.Specifications;

public sealed class GetNotificationTokenForObserverSpecification: SingleResultSpecification<NotificationToken>
{
    public GetNotificationTokenForObserverSpecification(Guid observerId)
    {
        Query.Where(x => x.ObserverId == observerId);
    }
}
