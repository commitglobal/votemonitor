using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.PollingStationAggregate;

namespace Feature.QuickReports.Specifications;

public sealed class GetPollingStationSpecification : SingleResultSpecification<PollingStation>
{
    public GetPollingStationSpecification(Guid electionRoundId, Guid pollingStationId)
    {
        Query.Where(x => x.ElectionRoundId == electionRoundId && x.Id == pollingStationId);
    }
}
