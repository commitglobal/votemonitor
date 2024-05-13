using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

namespace Feature.Forms.Specifications;

public sealed class GetMonitoringNgoSpecification : SingleResultSpecification<MonitoringNgo>
{
    public GetMonitoringNgoSpecification(Guid electionRoundId, Guid ngoId)
    {
        Query.Where(x => x.ElectionRoundId == electionRoundId && x.NgoId == ngoId);
    }
}
