using Vote.Monitor.Domain.Entities.ElectionRoundFormTemplateAggregate;

namespace Feature.FormTemplates.Specifications;

public sealed class ListElectionRoundFormTemplateSpecification : Specification<ElectionRoundFormTemplate>
{
    public ListElectionRoundFormTemplateSpecification(Guid electionRoundId)
    {
        Query.Where(e => e.ElectionRoundId == electionRoundId);
    }
}
