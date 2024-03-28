using Vote.Monitor.Api.Feature.Monitoring.GetMonitoringNgos;
using Vote.Monitor.Api.Feature.Monitoring.GetMonitoringObservers;

namespace Vote.Monitor.Api.Feature.Monitoring.Specifications;

public sealed class GetMonitoringNgoSpecification : SingleResultSpecification<MonitoringNgoAggregate, MonitoringNgoModel>
{
    public GetMonitoringNgoSpecification(Guid electionRoundId)
    {
        Query.Where(x => x.ElectionRoundId == electionRoundId);

        Query.Select(x => new MonitoringNgoModel
        {
            Id = x.Id,
            NgoId = x.NgoId,
            Name = x.Ngo.Name,
            NgoStatus = x.Ngo.Status,
            MonitoringNgoStatus = x.Status,
        });
    }
}

public sealed class GetMonitoringObserverSpecification : SingleResultSpecification<MonitoringObserverAggregate, MonitoringObserverModel>
{
    public GetMonitoringObserverSpecification(Guid electionRoundId, Guid monitoringNgoId)
    {
        Query.Where(x => x.InviterNgo.ElectionRoundId == electionRoundId && x.InviterNgoId == monitoringNgoId);

        Query.Select(x => new MonitoringObserverModel
        {
            Id = x.Id,
            ObserverId = x.ObserverId,
            InviterNgoId = x.InviterNgoId,
            Name = x.InviterNgo.Ngo.Name,
            Email = x.Observer.Login,
            PhoneNumber = x.Observer.PhoneNumber,
            MonitoringObserverStatus = x.Status,
            UserStatus = x.Observer.Status,
        });
    }
}
