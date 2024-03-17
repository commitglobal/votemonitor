using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;

namespace Vote.Monitor.Api.Feature.PollingStation.Notes.Specifications;

public sealed class GetElectionRoundSpecification : Specification<ElectionRound>
{
    public GetElectionRoundSpecification(Guid electionRoundId)
    {
        Query.Where(x => x.Id == electionRoundId);
    }
}
