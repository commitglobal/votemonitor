using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.CoalitionAggregate;

namespace Authorization.Policies.Specifications;

internal sealed class GetCoalitionLeaderDetailsSpecification : SingleResultSpecification<Coalition, CoalitionLeaderView>
{
    public GetCoalitionLeaderDetailsSpecification(Guid electionRoundId, Guid coalitionId, Guid ngoId)
    {
        Query
            .Include(x => x.ElectionRound)
            .Include(x => x.Leader)
            .Where(x => x.Leader.NgoId == ngoId && x.ElectionRoundId == electionRoundId && x.Id == coalitionId)
            .AsNoTracking();

        Query.Select(x => new CoalitionLeaderView
        {
            ElectionRoundId = x.ElectionRoundId,
            ElectionRoundStatus = x.ElectionRound.Status,
            NgoId = x.Leader.NgoId,
            NgoStatus = x.Leader.Ngo.Status,
            MonitoringNgoId = x.Id,
            MonitoringNgoStatus = x.Leader.Status
        });
    }
}
