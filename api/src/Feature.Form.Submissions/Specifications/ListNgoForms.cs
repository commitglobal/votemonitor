using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Feature.Form.Submissions.Specifications;

public sealed class ListNgoForms: Specification<FormAggregate>
{
    public ListNgoForms(Guid electionRoundId ,Guid ngoId)
    {
        Query
            .Where(x => x.ElectionRoundId == electionRoundId && x.MonitoringNgo.NgoId == ngoId)
            .Where(x => x.Status == FormStatus.Published);
    }
}
