using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.CoalitionAggregate;

namespace Feature.Form.Submissions.Specifications;

public sealed class GetCoalitionFormSpecification : SingleResultSpecification<Coalition, FormAggregate>
{
    public GetCoalitionFormSpecification(Guid electionRondId, Guid observerId, Guid formId)
    {
        Query.Where(x => x.ElectionRoundId == electionRondId)
            .Where(x => x.Memberships.Any(cm =>
                cm.MonitoringNgo.MonitoringObservers.Any(o => o.ObserverId == observerId)))
            .Where(x => x.FormAccess.Any(fa =>
                fa.MonitoringNgo.MonitoringObservers.Any(o => o.ObserverId == observerId) && fa.FormId == formId));

        Query.Include(x => x.FormAccess).ThenInclude(x => x.Form).ThenInclude(x => x.ElectionRound);

        Query.Select(x => x.FormAccess.First(f => f.FormId == formId).Form);
    }
}
