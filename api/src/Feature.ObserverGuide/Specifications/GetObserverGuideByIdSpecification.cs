using Ardalis.Specification;

namespace Feature.ObserverGuide.Specifications;

public sealed class GetObserverGuideByIdSpecification : SingleResultSpecification<ObserverGuideAggregate>
{
    public GetObserverGuideByIdSpecification(Guid electionRoundId, Guid ngoId, Guid id)
    {
        Query.Include(x => x.MonitoringNgo)
            .Where(x =>
                x.MonitoringNgo.ElectionRoundId == electionRoundId
                && x.MonitoringNgo.NgoId == ngoId
                && x.Id == id
                && !x.IsDeleted);
    }
}
