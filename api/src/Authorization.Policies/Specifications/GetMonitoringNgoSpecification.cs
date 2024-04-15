using Ardalis.Specification;

namespace Authorization.Policies.Specifications;

internal sealed class GetMonitoringNgoSpecification : SingleResultSpecification<MonitoringNgo, MonitoringNgoView>
{
    public GetMonitoringNgoSpecification(Guid electionRoundId, Guid ngoId)
    {
        Query
            .Include(x => x.ElectionRound)
            .Include(x => x.Ngo)
            .Where(x => x.NgoId == ngoId && x.ElectionRoundId == electionRoundId);

        Query.Select(x => new MonitoringNgoView
        {
            ElectionRoundId = x.ElectionRoundId,
            ElectionRoundStatus = x.ElectionRound.Status,
            NgoId = x.NgoId,
            NgoStatus = x.Ngo.Status,
            MonitoringNgoId = x.Id,
            MonitoringNgoStatus = x.Status
        });
    }
}
