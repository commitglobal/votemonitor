using Ardalis.Specification;

namespace Feature.ObserverGuide.Specifications;

public sealed class GetObserverGuidesSpecification : Specification<ObserverGuideAggregate>
{
    public GetObserverGuidesSpecification(Guid? ngoId)
    {
        Query.Include(x=> x.MonitoringNgo)
            .Where(x => x.MonitoringNgo.NgoId == ngoId && !x.IsDeleted);
    }
}
