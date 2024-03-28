using Ardalis.Specification;

namespace Feature.ObserverGuide.Specifications;

public sealed class GetObserverGuideSpecification : SingleResultSpecification<ObserverGuideAggregate>
{
    public GetObserverGuideSpecification(Guid? ngoId, Guid id)
    {
        Query.Where(x => x.MonitoringNgo.NgoId == ngoId && x.Id == id);
    }
}
