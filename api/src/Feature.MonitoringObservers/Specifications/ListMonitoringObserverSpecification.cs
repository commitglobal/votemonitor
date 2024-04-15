using Vote.Monitor.Domain.Specifications;

namespace Feature.MonitoringObservers.Specifications;

public sealed class ListMonitoringObserverSpecification : Specification<MonitoringObserverAggregate, MonitoringObserverModel>
{
    public ListMonitoringObserverSpecification(List.Request request)
    {
        Query
            .Search(x => x.Observer.ApplicationUser.FirstName, "%" + request.NameFilter + "%", !string.IsNullOrEmpty(request.NameFilter))
            .Search(x => x.Observer.ApplicationUser.LastName, "%" + request.NameFilter + "%", !string.IsNullOrEmpty(request.NameFilter))
            .Search(x => x.Observer.ApplicationUser.Email, "%" + request.EmailFilter + "%", !string.IsNullOrEmpty(request.EmailFilter))
            .Where(x => x.MonitoringNgo.ElectionRoundId == request.ElectionRoundId
                         && x.MonitoringNgoId == request.MonitoringNgoId)
            .Where(x => x.Tags.Intersect(request.Tags).Any(), request.Tags.Any())
            .Where(x => x.Status == request.Status, request.Status != null)
            .ApplyOrdering(request)
            .Paginate(request);

        Query
            .Include(x => x.Observer)
            .ThenInclude(x=>x.ApplicationUser);

        Query.Select(entity => MonitoringObserverModel.FromEntity(entity));
    }
}
