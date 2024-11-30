using Ardalis.Specification;

namespace Authorization.Policies.Specifications;

internal sealed class GetMonitoringObserverSpecification : SingleResultSpecification<MonitoringObserver, MonitoringObserverView>
{
    public GetMonitoringObserverSpecification(Guid electionRoundId, Guid? ngoId, Guid observerId)
    {
        Query
            .Include(x => x.MonitoringNgo)
            .ThenInclude(x => x.ElectionRound)
            .Where(x => x.ObserverId == observerId
                        && x.ElectionRoundId == electionRoundId 
                        && x.MonitoringNgo.NgoId == ngoId 
                        && x.MonitoringNgo.ElectionRoundId == electionRoundId);

        Query.Select(x => new MonitoringObserverView
        {
            ElectionRoundId = x.MonitoringNgo.ElectionRoundId,
            ElectionRoundStatus = x.MonitoringNgo.ElectionRound.Status,
            NgoId = x.MonitoringNgo.NgoId,
            NgoStatus = x.MonitoringNgo.Ngo.Status,
            MonitoringNgoId = x.MonitoringNgoId,
            MonitoringNgoStatus = x.MonitoringNgo.Status,
            ObserverId = x.ObserverId,
            UserStatus = x.Observer.ApplicationUser.Status,
            MonitoringObserverStatus = x.Status,
            MonitoringObserverId = x.Id
        });
    }

    public GetMonitoringObserverSpecification(Guid electionRoundId, Guid observerId)
    {
        Query
            .Include(x => x.MonitoringNgo)
            .ThenInclude(x => x.ElectionRound)
            .Where(x => x.ObserverId == observerId && x.ElectionRoundId == electionRoundId && x.MonitoringNgo.ElectionRoundId == electionRoundId)
            .AsNoTracking();

        Query.Select(x => new MonitoringObserverView
        {
            ElectionRoundId = x.MonitoringNgo.ElectionRoundId,
            ElectionRoundStatus = x.MonitoringNgo.ElectionRound.Status,
            NgoId = x.MonitoringNgo.NgoId,
            NgoStatus = x.MonitoringNgo.Ngo.Status,
            MonitoringNgoId = x.MonitoringNgoId,
            MonitoringNgoStatus = x.MonitoringNgo.Status,
            ObserverId = x.ObserverId,
            UserStatus = x.Observer.ApplicationUser.Status,
            MonitoringObserverStatus = x.Status,
            MonitoringObserverId = x.Id
        });
    }
}
