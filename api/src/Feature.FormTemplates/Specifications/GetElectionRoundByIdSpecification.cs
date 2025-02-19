using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;

namespace Feature.FormTemplates.Specifications;

public sealed class GetElectionRoundByIdSpecification : Specification<ElectionRound>
{
    public GetElectionRoundByIdSpecification(Guid electionRoundId)
    {
        Query.Where(x => x.Id == electionRoundId);
    }
}
