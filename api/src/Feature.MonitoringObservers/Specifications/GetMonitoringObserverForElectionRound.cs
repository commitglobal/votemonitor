namespace Feature.MonitoringObservers.Specifications;
public sealed class GetMonitoringObserverForElectionRound : SingleResultSpecification<MonitoringObserverAggregate>
{
    public GetMonitoringObserverForElectionRound(Guid electionRoundId, Guid observerId)
    {
        Query.Where(x => x.MonitoringNgo.ElectionRoundId == electionRoundId && x.ObserverId == observerId);
    }
}
