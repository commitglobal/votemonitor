using Ardalis.Specification;

namespace Feature.Form.Submissions.Specifications;

public sealed class GetMonitoringNgoFormSpecification : SingleResultSpecification<FormAggregate>
{
    public GetMonitoringNgoFormSpecification(Guid electionRondId, Guid observerId, Guid formId)
    {
        Query.Where(x =>
            x.ElectionRoundId == electionRondId
            && x.Id == formId &&
            x.MonitoringNgo.MonitoringObservers.Any(mo =>
                mo.ObserverId == observerId && mo.ElectionRoundId == electionRondId));
        Query.Include(x => x.ElectionRound);
    }
}
