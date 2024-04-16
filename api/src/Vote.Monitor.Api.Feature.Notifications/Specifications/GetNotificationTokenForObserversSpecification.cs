using Vote.Monitor.Domain.Entities.NotificationTokenAggregate;

namespace Vote.Monitor.Api.Feature.Notifications.Specifications;

public sealed class GetNotificationTokenForObserversSpecification: SingleResultSpecification<NotificationToken>
{
    public GetNotificationTokenForObserversSpecification(List<Guid> observerIds)
    {
        Query.Where(x => observerIds.Contains(x.ObserverId));
    }
}
