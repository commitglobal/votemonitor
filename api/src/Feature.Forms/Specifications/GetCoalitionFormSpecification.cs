using Vote.Monitor.Domain.Entities.CoalitionAggregate;

namespace Feature.Forms.Specifications;

public sealed class GetCoalitionFormSpecification : SingleResultSpecification<Coalition, FormAggregate>
{
    public GetCoalitionFormSpecification(Guid electionRondId, Guid ngoId, Guid formId)
    {
        Query.Where(x => x.ElectionRoundId == electionRondId)
            .Where(x => x.Memberships.Any(cm => cm.MonitoringNgo.NgoId == ngoId))
            .Where(x => x.FormAccess.Any(fa => fa.MonitoringNgo.NgoId == ngoId && fa.FormId == formId));

        Query.Include(x => x.FormAccess).ThenInclude(x => x.Form).ThenInclude(x => x.ElectionRound);

        Query.Select(x => x.FormAccess.First(f => f.FormId == formId).Form);
    }
}
