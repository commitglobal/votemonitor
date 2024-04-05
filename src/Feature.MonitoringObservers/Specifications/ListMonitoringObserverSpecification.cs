using Vote.Monitor.Domain.Specifications;

namespace Feature.MonitoringObservers.Specifications;

public sealed class ListMonitoringObserverSpecification : SingleResultSpecification<MonitoringObserverAggregate, MonitoringObserverModel>
{
    public ListMonitoringObserverSpecification(List.Request request)
    {
        Query.Where(x => x.MonitoringNgo.ElectionRoundId == request.ElectionRoundId
                         && x.MonitoringNgoId == request.MonitoringNgoId)
            .Where(x => x.Tags.Intersect(request.Tags).Any(), request.Tags.Any())
            .ApplyOrdering(request)
            .Paginate(request);

        Query.Select(x => new MonitoringObserverModel
        {
            Id = x.Id,
            ObserverId = x.ObserverId,
            InviterNgoId = x.MonitoringNgoId,
            Name = x.MonitoringNgo.Ngo.Name,
            Email = x.Observer.Login,
            PhoneNumber = x.Observer.PhoneNumber,
            MonitoringObserverStatus = x.Status,
            UserStatus = x.Observer.Status,
            Tags = x.Tags
        });
    }
}
